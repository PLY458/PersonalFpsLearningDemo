using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS_Movement_Control;
using System;
using Impact;
using Impact.Triggers;
using Random = UnityEngine.Random;

namespace FPS_Weapon_Control
{
    public enum E_Weapon_Status
    {
        Ready,
        Aiming,
        Swapping,
        TakingOut,
        Blocked,
        None

    }

    [RequireComponent(typeof(HandSway))]
    public class WeaponControl : SingletonMono<WeaponControl>
    {
        // 射线检测层级
        [SerializeField]
        private LayerMask Layer_Damage;

        // 武器状态
        [SerializeField]
        private E_Weapon_Status weaponStu;
        // 当前激活武器
        [SerializeField]
        private WeaponHandler handler_Current;
        // 武器集中特效
        [SerializeField]
        private ImpactTag bulletTag;

        // 武器运动特效
        HandSway weaponSwaying;

        // 武器序列相关
        List<int> autoReloadGun;

        public List<WeaponHandler> weaponInventory = new List<WeaponHandler>();
        public int selectedWeapon = -1;
        int putAwayWeapon = -1;
        // 
        [Range(1f, 2f)]
        public float sprintADSMultiplier = 1.25f;
        [Range(0f, 1f)]
        public float crouchADSMultiplier = 0.75f;

        // 输入内容
        [HideInInspector]
        public bool input_Fire;
        [HideInInspector]
        public bool input_Aim;
        [HideInInspector]
        public bool input_Aiming;
        [HideInInspector]
        public bool input_Reload;
        [HideInInspector]
        public float input_Scroll;

        // 武器性能数据
        float fireTimer = 0;

        // 是否长按瞄准
        public bool toggleADS = false;
        // 是否触发射击
        public bool triggerShooting = false;
        private int continuousShots = 0;
        int semiCalculations = 16;
        private bool onShootUp;
        // 武器散布
        private float bulletSpread = 0.01f;
        private float fireDelayTimer;

        // 确认是否瞄准
        public bool IsAiming { get {
                return weaponStu == E_Weapon_Status.Aiming;
            } }

        public bool IsBlocking { get {
                return (int)PlayController.GetInstance().move_Current >= 4;
            } }

        public bool IsReadyToFire {
            get {

                if (!handler_Current)
                    return false;
                if (handler_Current.ContFireMore)
                    return false;               
                if (IsBlocking)
                    return handler_Current.weapon.canFireWhileActing;
                else
                    return true;
            }
        }

        public float CurrentBulletSpread {
            get {
                float spread = handler_Current.weapon.bulletSpread;
                if (IsBlocking || PlayController.GetInstance().move_Current == E_Move_Status.Sprinting)
                    spread *= sprintADSMultiplier;
                else if (PlayController.GetInstance().move_Current == E_Move_Status.Crouching)
                    spread *= crouchADSMultiplier;

                return spread;
            }
        }

        public bool IsWeaponSelected {
            get {
                return (selectedWeapon >= 0 && selectedWeapon < weaponInventory.Count);
            }
        }


        private void Start()
        {
            weaponSwaying = GetComponent<HandSway>();

            EventCenter.GetInstance().AddEventListener<bool>("GetFireInput", (input) => input_Fire = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetAimInput", (input) => input_Aim = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetAimKeeper", (input) => input_Aiming = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetReloadInput", (input) => input_Reload = input);
            EventCenter.GetInstance().AddEventListener<float>("GetMouseScroll", (input) => input_Scroll = input);
            

            InitWeapons();
        }

        private void Update()
        {
            handler_Current = (IsWeaponSelected) ? weaponInventory[selectedWeapon] : null;

            AimingHandler();
            SpreadHandler();
            SwitchWeaponHandler();
            if ((int)weaponStu >= 2)
                return;

            MovetionHandler();
            FireWeaponHandler();
            ManualReloadHandler();
        }

        private void SwitchWeaponHandler()
        {
            if (triggerShooting || IsBlocking) return;
            if (IsWeaponSelected)
            {
                float scroll = input_Scroll;
                if (scroll >= 0.1f)
                    ChangeSelectedGun(1);
                else if (scroll <= -0.1f)
                    ChangeSelectedGun(-1);
            }
        }

        private void ChangeSelectedGun(int add)
        {

            if (weaponStu != E_Weapon_Status.Swapping)
            {
                //source.Stop(); //Stop reloading SFX if playing
                AudioMgr.GetInstance().StopSound("Weapon");
                AudioMgr.GetInstance().PlaySound("Weapon", "Select");
                //fireDelayTimer = 0; //Reset the fire delay
                weaponInventory[selectedWeapon].PutAwayWeapon(TakeOutSelectedGun);
                weaponStu = E_Weapon_Status.Swapping;
                putAwayWeapon = selectedWeapon;
            }

            selectedWeapon += add;
            if (selectedWeapon < 0)
                selectedWeapon += weaponInventory.Count;
            else if (selectedWeapon >= weaponInventory.Count)
                selectedWeapon -= weaponInventory.Count;
        }

        public void TakeOutSelectedGun()
        {
            if (putAwayWeapon >= 0)
            {
                weaponInventory[putAwayWeapon].gameObject.SetActive(false);
                putAwayWeapon = -1;
            }

            EventCenter.GetInstance().EventTrigger("SetWeapon_weaponBar", handler_Current);
            handler_Current.InitHandler();
            weaponInventory[selectedWeapon].gameObject.SetActive(true);
            weaponStu = E_Weapon_Status.TakingOut;
        }

        public void TakeWeaponOut(int reloadIndex)
        {
            
            weaponStu = E_Weapon_Status.Ready;
            if (reloadIndex >= 0)
                AutoReloadGun(reloadIndex);
        }


        private void InitWeapons()
        {
            autoReloadGun = new List<int>();

            weaponStu = E_Weapon_Status.Ready;
            // 武器序列初始化
            foreach (var w in weaponInventory)
            {

                w.gameObject.SetActive(false);
            }
                
            if (IsWeaponSelected)
            {
                weaponInventory[selectedWeapon].InitHandler();
                weaponInventory[selectedWeapon].gameObject.SetActive(true);
                EventCenter.GetInstance().EventTrigger("SetWeapon_weaponBar", weaponInventory[selectedWeapon]);
            }

            //EventCenter.GetInstance().EventTrigger("SetWeapon_weaponBar", handler_Current);
        }

        // 运动状态切换
        void MovetionHandler()
        {
            if (handler_Current)
            {
                int move = (int)PlayController.GetInstance().move_Current;

                switch (move) {
                    case (int)E_Move_Status.Sprinting:
                        handler_Current.UpdateSprint(true);
                        handler_Current.UpdateAim(false);
                        handler_Current.UpdateWalk(false);
                        break;
                    case (int)E_Move_Status.Walking:
                        handler_Current.UpdateSprint(false);
                        if (!IsAiming)
                        {
                            handler_Current.UpdateWalk(true);
                            handler_Current.UpdateAim(false);
                        }
                        else
                        {
                            handler_Current.UpdateWalk(true);
                            handler_Current.UpdateAim(true);
                        }
                        break;
                    case (int)E_Move_Status.Idle:
                        handler_Current.UpdateAim(IsAiming);
                        handler_Current.UpdateWalk(false);
                        handler_Current.UpdateSprint(false);
                        break;
                    case (int)E_Move_Status.Crouching:
                        handler_Current.UpdateAim(IsAiming);
                        handler_Current.UpdateWalk(false);
                        handler_Current.UpdateSprint(false);
                        break;
                    default:
                        handler_Current.UpdateAim(false);
                        handler_Current.UpdateWalk(false);
                        handler_Current.UpdateSprint(false);
                        break;
                }

            }
                
        }

        // 瞄准状态切换
        void AimingHandler()
        {
            if (handler_Current)
            {
                // 在武器待机和瞄准时
                if ((int)weaponStu < 2)
                {
                    int move = (int)PlayController.GetInstance().move_Current;

                    if ((int)handler_Current.handlerStu < 1 && !IsBlocking && move != (int)E_Move_Status.Sprinting)
                    {
                        
                        if (toggleADS && input_Aim)
                            weaponStu = (weaponStu == E_Weapon_Status.Aiming) ? E_Weapon_Status.Ready : E_Weapon_Status.Aiming;
                        else if (!toggleADS)
                            weaponStu = (input_Aiming) ? E_Weapon_Status.Aiming : E_Weapon_Status.Ready;
                    }
                    else
                        weaponStu = E_Weapon_Status.Ready;
                }

                AdjustFOV(IsAiming);
                //gunHandler.AimDownSights(isAiming());
                //if (ui) ui.SetCrosshair(isAiming() ? 0.01f : bulletSpread, isAiming());
                EventCenter.GetInstance().EventTrigger("SetCrossHair_crossHair", IsAiming ? 0.01f : bulletSpread);
                weaponSwaying.SetSwayMultiplier(IsAiming ? handler_Current.weapon.aimSpreadMultiplier : 1f);
            }
            else
            {
                weaponStu = E_Weapon_Status.None;
                EventCenter.GetInstance().EventTrigger("SetCrossHair_crossHair", 0.01f);
                //if (ui) ui.SetCrosshair(0.01f, true);
            }
        }

        private void AdjustFOV(bool aiming)
        {
            WeaponObject curWeapon = handler_Current.weapon;
            float fov = curWeapon.aimFOV;
            PlayController.GetInstance().movement_Camera.SetFOV(aiming, curWeapon.aimFOV, curWeapon.aimDownSpeed);
        }

        void SpreadHandler()
        {
            if (handler_Current)
            {
                WeaponObject weapon = handler_Current.weapon;
                float actualSpread = bulletSpread;
                float spread = CurrentBulletSpread;
                if (IsAiming)
                {
                    spread *= weapon.aimSpreadMultiplier;
                    actualSpread = Mathf.Lerp(actualSpread, spread, Time.deltaTime * weapon.aimDownSpeed * (1f - weapon.aimDownMultiplier));
                }
                else
                    actualSpread = Mathf.Lerp(actualSpread, spread, Time.deltaTime * 4f);

                //if (handler_Current.weapon.canFireWhileDelayed)
                //{
                //    float delaySpreadAdjust = (4f - (DelayPercent() * 3f)) / 4f;
                //    actualSpread = Mathf.Lerp(gunHandler.gun.bulletSpread, (spread * delaySpreadAdjust), DelayPercent());
                //}
                bulletSpread = actualSpread;
            }
            else
                bulletSpread = 0.01f;
        }

        void FireDelayHandler()
        {
            if (!handler_Current)
            {
                fireDelayTimer = 0;
                return;
            }

            float adjust = (IsReadyToFire && input_Fire) ? Time.deltaTime : -(Time.deltaTime * handler_Current.weapon.fireCooldownSpeed);
            fireDelayTimer = Mathf.Clamp(fireDelayTimer + adjust, 0, handler_Current.weapon.fireDelay);
        }

        void FireWeaponHandler()
        {
            WeaponObject weapon = handler_Current.weapon;
            //HandleOnShootUp();

            //handler_Current.OnDelayCall(fireDelayTimer);

            Transform camera = PlayController.GetInstance().movement_Camera.transform;

            // 探测可伤害物体血量
            //Ray tempRay = new Ray(camera.position, camera.forward);

            //if (Physics.Raycast(tempRay, out var check, 10f, Layer_Damage))
            //{
            //    float dis = Vector3.Distance(camera.position, check.point) + 0.05f;
            //    if(Physics.Raycast(tempRay, out var target, dis, Layer_Damage))
            //    {
            //        DamageZone tempZone = target.transform.GetComponent<DamageZone>();

            //        if (tempZone != null)
            //        {
            //            if (!tempZone.IsDamageObjDied())
            //            {
            //                EventCenter.GetInstance().EventTrigger("SetHealthBar_crossHair", tempZone.DamgeObj);
            //            }

            //        }
            //    }

            //}


                // 开火速率控制
                if (fireTimer > 0)
                fireTimer -= Time.deltaTime;
            else if (handler_Current)
            {
                if (!triggerShooting)
                {
                    // 优先启动还未完成自动装填的枪械序列
                    if (autoReloadGun.Count > 0 && autoReloadGun.Contains(handler_Current.weaponIndex))
                    {
                        if (IsBlocking)
                            return;
                        if (!handler_Current.ReadyToReload)
                            return;
                        if (handler_Current.handlerStu == E_Handler_Status.Reloading)
                            return;

                        if (ReloadSelectedGun())
                            autoReloadGun.Remove(handler_Current.weaponIndex);
                        return;
                    }

                    //if (fireDelayTimer < gunHandler.gun.fireDelay)
                    //{
                    //    if (!weapon.canFireWhileDelayed) return;
                    //}

                    if (!IsReadyToFire)
                    {
                        //if(input_Fire)
                        //    AudioMgr.GetInstance().PlaySound("Weapon", "Foley");

                        return;
                    }
                            

                    if (weapon.fireModel == E_FireModel.Auto && input_Fire)
                    {
                        continuousShots++;
                        FireWeapon(weapon);
                    }
                    else
                    {
                        bool fire = false;
                        //if (weapon.fireWhenPressedUp)
                        //{
                        //    if (onShootUp)
                        //        fire = true;
                        //}
                        //else if (input_Fire)
                        //    fire = true;

                        if (input_Fire)
                            fire = true;

                        if (fire)
                        {
                            continuousShots = 0;
                            FireWeapon(weapon);
                        }
                    }
                }
            }
        }

        private void FireWeapon(WeaponObject weapon)
        {

            triggerShooting = true;

            fireTimer = weapon.fireRate;

            switch (weapon.fireModel)
            {
                case E_FireModel.Semi:
                    AudioMgr.GetInstance().PlaySound("Weapon", "Fire");
                    float addTime = weapon.fireRate / (float)semiCalculations;
                    StartCoroutine(singleShot());
                    if (!handler_Current.FireWeapon())
                        AutoReloadGun(handler_Current.weaponIndex);
                    IEnumerator singleShot()
                    {
                        SimulateShot();
                        for (int i = 0; i < semiCalculations; i++)
                        {
                            continuousShots++;
                            ApplyRecoil(addTime);
                            yield return new WaitForSeconds(addTime);
                        }
                        triggerShooting = false;
                    }
                    fireDelayTimer = 0; //Restart the timer if semi or burst
                    break;
                case E_FireModel.Auto:
                    AudioMgr.GetInstance().PlaySound("Weapon","Fire");
                    //PlayShotSFX();
                    SimulateShot();
                    ApplyRecoil(weapon.fireRate);
                    if (!handler_Current.FireWeapon())
                    {
                        AutoReloadGun(handler_Current.weaponIndex);
                    }
                    //
                    triggerShooting = false;
                    break;
                case E_FireModel.Burst:
                    //PlayShotSFX();
                    //float shotTime = gun.burstTime / (float)gun.burstShot;
                    //StartCoroutine(burstShot());
                    //IEnumerator burstShot()
                    //{
                    //    for (int i = 0; i < gun.burstShot; i++)
                    //    {
                    //        SimulateShot();
                    //        continuousShots++;
                    //        ApplyRecoil(shotTime);
                    //        if (!gunHandler.ShootGun())
                    //        {   //Stop when we run out of bullets
                    //            AutoReloadGun(gunHandler.gunIndex);
                    //            yield return null;
                    //        }
                    //        else
                    //            yield return new WaitForSeconds(shotTime);
                    //    }
                    //    shootingGun = false;
                    //}
                    //fireDelayTimer = 0; //Restart the timer if semi or burst
                    break;
            }
        }

        private void ApplyRecoil(float firerate)
        {
            WeaponObject weapon = handler_Current.weapon;

            float aimAdjust = (IsAiming) ? weapon.aimDownMultiplier : 1f;

            Vector3 shotRecoil = Vector3.zero;
            if (weapon.fireModel == E_FireModel.Auto)
                shotRecoil = weapon.recoilInfo.GetRecoil(continuousShots, weapon.ammoClip, weapon.cyclesInClip);
            //else if (gun.shooting == GunObject.ShootType.burst)
            //    shotRecoil = gun.recoil.GetRecoil(continuousShots, gun.burstShot, 1);
            else if (weapon.fireModel == E_FireModel.Semi)
                shotRecoil = weapon.recoilInfo.GetRecoil(continuousShots, semiCalculations);

            shotRecoil *= aimAdjust;
            PlayController.GetInstance().movement_Camera.AddRecoil(shotRecoil, firerate);

            shotRecoil.y = 0;
            float z = shotRecoil.z;
            shotRecoil.x = weapon.recoilInfo.xRecoil.EvaluteValue(Random.Range(0.0f, 1.0f));
            shotRecoil.x *= aimAdjust;
            shotRecoil *= firerate;
            shotRecoil.z = z;

            GunRecoil(shotRecoil, firerate);
        }

        void GunRecoil(Vector3 recoil, float time)
        {
            float recoilElapsed = 0;
            StartCoroutine(recoilIncrease());
            IEnumerator recoilIncrease()
            {
                while (recoilElapsed < time)
                {
                    recoilElapsed += Time.deltaTime;
                    handler_Current.AddRecoil(recoil * Time.deltaTime);
                    yield return null;
                }
            }
        }

        private void SimulateShot()
        {
            WeaponObject weapon = handler_Current.weapon;

            if (weapon.shotModel == E_ShotModel.Raycast)
            {
                for (int i = 0; i < weapon.bulletsPerShot; i++)
                    RaycastShot();
            }
        }

        Vector3 GetShotDir(float spread)
        {
            Vector3 shotDir = Vector3.zero;
            shotDir.x = Random.Range(-spread, spread);
            shotDir.y = Random.Range(-spread, spread);
            shotDir.z = 1f;
            return shotDir;
        }

        void RaycastShot()
        {
            Transform camera = PlayController.GetInstance().movement_Camera.transform; //Shoot from camera, not gun

            WeaponObject weapon = handler_Current.weapon;

            Vector3 shotDir = GetShotDir(bulletSpread);
            Vector3 worldDir = camera.TransformDirection(shotDir);
            Vector3 impactPos = camera.position;
            impactPos += worldDir * weapon.bulletRange;

            Ray shotRay = new Ray(camera.position, worldDir);
            if (Physics.Raycast(shotRay, out var hit, weapon.bulletRange, PlayController.GetInstance().layer_moveCollsion))
            {

                float dis = Vector3.Distance(camera.position, hit.point) + 0.05f;


                if (Physics.Raycast(shotRay, out var dmg, dis, Layer_Damage))
                {
                    Debug.DrawLine(shotRay.origin, dmg.point, Color.cyan, 2f, true);

                    InteractionData data = new InteractionData()
                    {
                        Velocity = camera.forward * dis,
                        CompositionValue = 1,
                        PriorityOverride = 100,
                        ThisObject = gameObject,
                        TagMask = bulletTag.GetTagMask()
                    };

                    ImpactRaycastTrigger.Trigger(data, dmg, false);

                    // 1. 获取伤害区域
                    DamageZone tempZone = dmg.transform.GetComponent<DamageZone>();
                    if (tempZone != null)
                    {
                        // 2. 获取伤害区域父物体状态，若还未消灭则对齐造成伤害
                        if (!tempZone.IsDamageObjDied())
                        {
                            TargetController.GetInstance().SetDamage(tempZone, weapon.bulletDamage, weapon.headshotMult);
                            EventCenter.GetInstance().EventTrigger("SetHitMarker_crossHair", tempZone.IsDamageObjDied());
                        }
                    }
                    //If we hit something we should damage
                    //{  
                    //    if (!damaged.DamageableAlreadyDead())
                    //    {
                    //        bool killed = damaged.Damage(gun.bulletDamage, gun.headshotMult);
                    //        if (ui) ui.ShowHitmarker(killed); //Damages and shows hitmarker
                    //    }
                }
            }

            // 弹道轨迹生成
            //Vector3[] pos = { gunHandler.bulletSpawn.transform.position, impactPos };
            //ShotTrailHelper trail = helper.getAvailableTrail();
            //trail.Initialize(pos);
        }


        private void HandleOnShootUp()
        {
            throw new NotImplementedException();
        }

        void ManualReloadHandler()
        {
            if (triggerShooting)
                return; //If we are shooting, do nothing
            if (!handler_Current)
                return; //If we don't have a gun, do nothing
            if (IsBlocking)
                return;

            if (!input_Reload)
                return; //If we don't press R this frame, do nothing
            if (handler_Current.ReadyToReload)
                ReloadSelectedGun();
        }

        private bool ReloadSelectedGun()
        {
            WeaponObject weapon = handler_Current.weapon;
            AudioMgr.GetInstance().PlaySound("Weapon", "Reload");
            //if (gun.reloadSFX != null)
            //{
            //    source.clip = gun.reloadSFX;
            //    source.time = gun.reloadSFX.length * gunHandler.getReloadStartPoint();
            //    source.Play();
            //}
            return handler_Current.ReloadGun();

        }

        private void AutoReloadGun(int weaponIndex)
        {
/*         if (gunInventory[index].gun.startingClips < 0) return;*/ //We won't reload a gun that cannot reload

            if (!autoReloadGun.Contains(weaponIndex))
                autoReloadGun.Add(weaponIndex);
        }

        public void RefillAmmo()
        {
            foreach (var handler in weaponInventory)
            {
                var gun = handler.weapon;
                if (gun.startingClips >= 0)
                    handler.totalAmmo = gun.ammoClip * gun.startingClips;
                else
                    handler.ammoInClip = gun.ammoClip;
            }
        }
    }


}

