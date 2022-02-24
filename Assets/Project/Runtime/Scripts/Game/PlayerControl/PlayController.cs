using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS_Movement_Control
{
    public enum E_Move_Status
    {
        Idle,
        Walking,
        Crouching,
        Sprinting
    }


    public class PlayController : SingletonMono<PlayController>
    {
        // ��ײ�㼶����
        public LayerMask layer_Collsion;

        // ��ǰ�˶�״̬
        public E_Move_Status status_Current;

        private bool trigger_Interact;

        // �������������
        [HideInInspector]
        public Vector3 input_HoriDir;
        [HideInInspector]
        public bool input_IsSprint;
        [HideInInspector]
        public bool input_IsCrouching;
        [HideInInspector]
        public bool input_IsJumping;

        private PlayerMovement movement_Player;

        void Start()
        {
            trigger_Interact = false;

            // ע����������Ӧ���¼�
            EventCenter.GetInstance().AddEventListener<Vector3>("GetMoveDirInput", (input) => input_HoriDir = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetMoveRunInput", (input) => input_IsSprint = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetCrouchInput", (input) => input_IsCrouching = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetJumpInput", (input) => input_IsJumping = input);

            movement_Player = GetComponent<PlayerMovement>();
            PlayerInput.GetInstance().InitInput();
            PlayerInput.GetInstance().StartOrEndCheck(true);
        }

        #region ״̬�������

        public void SwitchStatus(E_Move_Status stu)
        {
            if (status_Current == stu)
                return;
            status_Current = stu;
        }


        // Update is called once per frame
        void Update()
        {
            
        }

        /// <summary>
        /// ����Ƿ�������彻��
        /// </summary>
        private void UpdateInteraction()
        {
            if ((int)status_Current >= 5)
                trigger_Interact = false;
            else if (!trigger_Interact)
            {
                // TODO: ������˶�����л�ȡ�����������Ƿ��ڵ�/��׹
                //if (movement.grounded || movement.moveDirection.y < 0)
                    trigger_Interact = true;
            }
        }

        /// <summary>
        /// ������ҵ�ǰ�˶�״̬
        /// </summary>
        private void UpdateMovingStatus()
        {

        }

        #endregion

        #region ״̬�ж����


        #endregion

        #region ���봦�����



        #endregion

    }

}


