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
        // 碰撞层级设置
        public LayerMask layer_Collsion;

        // 当前运动状态
        public E_Move_Status status_Current;

        private bool trigger_Interact;

        // 处理输入的数据
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

            // 注册玩家输入对应的事件
            EventCenter.GetInstance().AddEventListener<Vector3>("GetMoveDirInput", (input) => input_HoriDir = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetMoveRunInput", (input) => input_IsSprint = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetCrouchInput", (input) => input_IsCrouching = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetJumpInput", (input) => input_IsJumping = input);

            movement_Player = GetComponent<PlayerMovement>();
            PlayerInput.GetInstance().InitInput();
            PlayerInput.GetInstance().StartOrEndCheck(true);
        }

        #region 状态更新相关

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
        /// 检测是否可与物体交互
        /// </summary>
        private void UpdateInteraction()
        {
            if ((int)status_Current >= 5)
                trigger_Interact = false;
            else if (!trigger_Interact)
            {
                // TODO: 从玩家运动组件中获取两个条件：是否在地/下坠
                //if (movement.grounded || movement.moveDirection.y < 0)
                    trigger_Interact = true;
            }
        }

        /// <summary>
        /// 更新玩家当前运动状态
        /// </summary>
        private void UpdateMovingStatus()
        {

        }

        #endregion

        #region 状态判定相关


        #endregion

        #region 输入处理相关



        #endregion

    }

}


