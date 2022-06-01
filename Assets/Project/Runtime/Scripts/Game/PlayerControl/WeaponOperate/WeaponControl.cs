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
        // ���߼��㼶
        [SerializeField]
        private LayerMask Layer_Damage;

        // ����״̬
        [SerializeField]
        private E_Weapon_Status weaponStu;
        // ��ǰ��������
        [SerializeField]
        private WeaponHandler handler_Current;

        // ��������
        [HideInInspector]
        public bool input_Fire;
        [HideInInspector]
        public bool input_Aim;
        [HideInInspector]
        public bool input_Reload;

        // ������������
        float fireTimer = 0;

        // ȷ���Ƿ���׼
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
            // �������г�ʼ��

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

