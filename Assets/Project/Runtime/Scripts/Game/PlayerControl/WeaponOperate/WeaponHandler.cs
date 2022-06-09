using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FPS_Weapon_Control
{

    public enum E_Handler_Status
    {
        Idle, Reloading, PutAway
    }

    public class WeaponHandler : MonoBehaviour
    {

        public WeaponObject weapon;
        // �ӵ����ɵ�
        public Transform bulletSpwanPoint;

        // �����ַ�
        public WeaponAnimations motions;

        public WeaponFactory effectFactory;

        [Header("����״̬")]
        public E_Handler_Status handlerStu;
        public int weaponIndex = 0;

        private Animator handlrAnimator;

        [Header("ǹе��ҩ")]
        public int ammoInClip;
        public int totalAmmo;

        bool forceReload = false;
        private float reloadStartTime = default;

        // ���������
        Vector3 aimPos;
        Vector3 recoil;
        public float adjustSpeed = 3; // ǹе��λ�ٶ�

        private bool NoAmmo
        {
            get{ return (totalAmmo <= 0 && ammoInClip <= 0); }
        }

        public bool ContFireMore
        {
            get { return (handlerStu == E_Handler_Status.Reloading || NoAmmo); }
        }

        public bool ReadyToReload
        {
            get { return ((int)handlerStu < 2 && ammoInClip < weapon.ammoClip && totalAmmo > 0);  }
        }


        private void Start()
        {
            InitHandler();
        }

        public void InitHandler()
        {
            effectFactory.FirePrefab = weapon.fireVfx;
            handlrAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            RefreshHandler();

        }

        public void AddRecoil(Vector3 r)
        {
            recoil += (WeaponControl.GetInstance().IsAiming) ? (r / 2f) : r;
        }

        private void RefreshHandler()
        {
            // ���������ֵ���
            transform.localRotation = Quaternion.Slerp(transform.localRotation,
                (WeaponControl.GetInstance().IsAiming) ? Quaternion.Euler(weapon.sightAimTrans.eulerAngles) : Quaternion.identity,
                Time.deltaTime * 4f);

            aimPos = Vector3.Lerp(aimPos, (WeaponControl.GetInstance().IsAiming) ? weapon.sightAimTrans.position : Vector3.zero, Time.deltaTime * weapon.aimDownSpeed);
            recoil = Vector3.Lerp(recoil, Vector3.zero, Time.deltaTime * adjustSpeed);

            transform.localPosition = aimPos + recoil;
        }

        #region ������������

        public bool FireWeapon()
        {
            FireEffect tEffect = effectFactory.FireVfx;
            tEffect.SpawnOn(bulletSpwanPoint);
            AudioMgr.GetInstance().PlaySound("Weapon", "Gun");
            handlrAnimator.Play(motions.Shoot(WeaponControl.GetInstance().IsAiming),0,0);

            if (--ammoInClip <= 0)
                return false;
            else
                return true;
                
        }

        public void PutAwayWeapon()
        {
            forceReload = (handlerStu == E_Handler_Status.Reloading || NoAmmo);
            handlerStu = E_Handler_Status.PutAway;
            handlrAnimator.Play(motions.putAway, 0, 0);

            // ������control��ע��Ļص��¼�
            //if (listener)
            //    listener.onPutAway.Invoke();

            //onPutAway.RemoveAllListeners();
            //onPutAway.AddListener(call);
        }

        public void ReloadAmmo()
        {
            //reloadStartPoint = 0;
            handlerStu = E_Handler_Status.Idle;
            totalAmmo += ammoInClip;
            totalAmmo -= weapon.ammoClip;
            ammoInClip = weapon.ammoClip;
            if (totalAmmo < 0)
            {
                ammoInClip += totalAmmo;
                totalAmmo = 0;
            }

            //if (listener) listener.onReload.Invoke();
        }

        #endregion

        #region �����¼�������


        public bool ReloadGun()
        {
            if (ContFireMore)
                return false;

            handlerStu = E_Handler_Status.Reloading;
            handlrAnimator.Play(motions.reload, -1, reloadStartTime);

            //if (weapon.looseAmmoOnReload)
            //    ammoInClip = 0;

            return true;
        }




        public bool UpdateSprint(bool playerSprint)
        {
            if (handlerStu != E_Handler_Status.Idle)
            {
                handlrAnimator.SetBool("sprinting", false);
                return false;
            }
            else
            {
                handlrAnimator.SetBool("sprinting", playerSprint);
                return true;
            }


        }

        public bool UpdateWalk(bool playerWalk)
        {
            if (handlerStu != E_Handler_Status.Idle)
            {
                handlrAnimator.SetBool("walking", false);
                return false;
            }
            else
            {
                handlrAnimator.SetBool("walking", playerWalk);
                return true;
            }

        }

        public bool UpdateAim(bool playerAim)
        {
            if (handlerStu != E_Handler_Status.Idle)
            {
                handlrAnimator.SetBool("aiming", false);
                return false;
            }
            else
            {
                handlrAnimator.SetBool("aiming", playerAim);
                return true;
            }

        }




        #endregion

    }

    [System.Serializable]
    public class WeaponAnimations
    {
        public string idle = "idle";
        public string hipfireShoot = "shoot";
        public string aimingIdle = "aimingIdle";
        public string aimedShoot = "aimedShoot";
        public string reload = "reload";
        public string putAway = "putAway";
        public string takeOut = "takeOut";

        public WeaponAnimations()
        {
        }

        public WeaponAnimations(string i, string h, string aI ,string aS, string r, string p, string t)
        {
            idle = i;
            hipfireShoot = h;
            aimingIdle = aI;
            aimedShoot = aS;
            reload = r;
            putAway = p;
            takeOut = t;
        }

        public void SetAnimations(WeaponAnimations ani)
        {
            idle = ani.idle;
            hipfireShoot = ani.hipfireShoot;
            aimingIdle = ani.aimingIdle;
            aimedShoot = ani.aimedShoot;
            reload = ani.reload;
            putAway = ani.putAway;
            takeOut = ani.takeOut;
        }

        public string Shoot(bool aiming)
        {
            return (aiming) ? aimedShoot : hipfireShoot;
        }
    }
}



