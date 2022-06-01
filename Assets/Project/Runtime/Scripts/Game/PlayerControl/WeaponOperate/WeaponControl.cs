using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS_Movement_Control;

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

        // 输入内容
        [HideInInspector]
        public bool input_Fire;
        [HideInInspector]
        public bool input_Aim;
        [HideInInspector]
        public bool input_Reload;

        // 武器性能数据
        float fireTimer = 0;

        // 确认是否瞄准
        public bool IsAiming { get { return weaponStu == E_Weapon_Status.Aiming; } }

        private void Start()
        {
            EventCenter.GetInstance().AddEventListener<bool>("GetFireInput", (input) => input_Fire = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetAimInput", (input) => input_Aim = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetReloadInput", (input) => input_Reload = input);


        }

        private void Update()
        {
                
        }

        private void InitWeapons()
        {
            weaponStu = E_Weapon_Status.Ready;
            // 武器序列初始化

        }

        void MovetionHandler()
        {
            if (handler_Current)
            {
                handler_Current.UpdateAim(IsAiming);

                int move = (int)PlayController.GetInstance().move_Current;

            }
                
        }


    }


}

