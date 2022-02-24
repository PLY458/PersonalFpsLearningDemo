using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FPS_Movement_Control
{
    /// <summary>
    /// 玩家运动的核心基底
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : InterpolatedTransform
    {
        // 速度相关数据
        public float speed_Walk = 4.0f;
        public float speed_Sprint = 8.0f;
        public float speed_Crouch = 2f;
        [SerializeField]
        private float speed_Jump= 8.0f;
        // 重力相关数据
        [SerializeField]
        private float force_Gravity = 20.0f;
        [SerializeField]
        private float factor_AntiBump = .75f;

        [HideInInspector]
        public Vector3 dir_GroundMove = Vector3.zero;
        [HideInInspector]
        public Vector3 dir_AirMove = Vector3.zero;
        [HideInInspector]
        public Vector3 pos_Contact;

        public bool Is_Grounded { get => is_Grounded;  }
        private bool is_Grounded = false;

        private Vector3 input_Jump = Vector3.zero;

        private bool trigger_EnforceGravity;
        private float time_Enforce = 0f;
        private float force_Jump = 0f;

        // 跳跃计时器和判定
        private int timer_Jump;

        // 冲刺相关数据
        [SerializeField]
        private float time_Sprint = 6f;
        [SerializeField]
        private float reserve_Sprint = 4f;
        [SerializeField]
        private float miniLess_Sprint = 2f;
        // 冲刺耐力
        private float stamina;
        // 状态缓存
        private E_Move_Status t_status;

        // 蹲俯相关数据
        [SerializeField]
        private float height_Crouch = 1f;
        [SerializeField]
        private float height_Jump = 1f;

        [HideInInspector]
        public CharacterController controller;
        [HideInInspector]
        public PlayerIntractInfo info_Player;


        private void Start()
        {
            timer_Jump = -1;
            controller = GetComponent<CharacterController>();
            info_Player = new PlayerIntractInfo(controller.radius, controller.height, height_Crouch);
            stamina = time_Sprint;
        }

        public override void Update()
        {
            Vector3 newestTransform = m_lastPositions[m_newTransformIndex];
            Vector3 olderTransform = m_lastPositions[OldTransformIndex()];

            Vector3 adjust = Vector3.Lerp(olderTransform, newestTransform, InterpolationController.InterpolationFactor);
            adjust -= transform.position;

            controller.Move(adjust);

            if (time_Enforce > 0)
                time_Enforce -= Time.deltaTime;

            UpdateMovingStatus();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (time_Enforce > 0)
            {
                if (trigger_EnforceGravity)
                    dir_GroundMove.y -= force_Gravity * Time.deltaTime;
                is_Grounded = (controller.Move(dir_GroundMove * Time.deltaTime) & CollisionFlags.Below) != 0;
            }

            //注入默认移动状态
            UpdateGlobalMovement();
        }

        private void UpdateGlobalMovement()
        {
            GenaralGroundMove();

            CheckCrouchAct();
            CheckJumpAct();
        }

        private void CheckJumpAct()
        {
            // 更新跳跃
            bool jump = PlayController.GetInstance().input_IsJumping;
            if (!jump)
            {
                timer_Jump++;
                return;
            }
            else if (timer_Jump > 0)
            {
                input_Jump = Vector3.up * height_Jump;
                timer_Jump = -1;
            }
            UpdateJump();
        }

        public void UpdateJump()
        {
            if(input_Jump != Vector3.zero)
            {
                Vector3 dir = (input_Jump * speed_Jump);
                if (dir.x != 0) dir_GroundMove.x = dir.x;
                if (dir.y != 0) dir_GroundMove.y = dir.y;
                if (dir.z != 0) dir_GroundMove.z = dir.z;

                Vector3 move = dir_GroundMove;
                dir_AirMove = move;
                move.y = 0;
                force_Jump = Mathf.Min(move.magnitude, speed_Jump);
                force_Jump = Mathf.Max(force_Jump, speed_Walk);
            }
            else
                dir_AirMove = Vector3.zero;
            input_Jump = Vector3.zero;
        }

        private void UpdateMovingStatus()
        {
            t_status = PlayController.GetInstance().status_Current;
            // 对冲刺耐久值计算
            if (t_status == E_Move_Status.Sprinting && stamina > 0)
                stamina -= Time.deltaTime;
            else if (stamina < time_Sprint)
                stamina += Time.deltaTime;

            // 站立或冲刺状态时判定下个运动状态
            if ((int)t_status <= 1 || InSprinting())
            {
                Vector2 input = PlayController.GetInstance().input_HoriDir;
                if (input.magnitude > 0.02f)
                    PlayController.GetInstance().SwitchStatus(ReadyToSprinting() ? 
                        E_Move_Status.Sprinting : E_Move_Status.Walking);
                else
                    PlayController.GetInstance().SwitchStatus(E_Move_Status.Idle);
            }



        }

        // 默认情况下的地表移动操作
        public void GenaralGroundMove()
        {
            Vector2 dir = PlayController.GetInstance().input_HoriDir;

            if (time_Enforce > 0)
                return;

            float nextSpeed = 0f;
            if (InCrouching())
                nextSpeed = speed_Crouch;
            else
                nextSpeed = (!InSprinting()) ? speed_Walk : speed_Sprint;

            if (is_Grounded)
            {
                dir_GroundMove = new Vector3(dir.x, -factor_AntiBump, dir.y);
                dir_GroundMove = transform.TransformDirection(dir_GroundMove) * nextSpeed;
                //UpdateJump();
            }
            else
            {
                Vector3 adjust = new Vector3(dir.x, 0, dir.y);
                adjust = transform.TransformDirection(adjust);
                dir_AirMove += adjust * Time.fixedDeltaTime * force_Jump * 2f;
                dir_AirMove = Vector3.ClampMagnitude(dir_AirMove, force_Jump);
                dir_GroundMove.x = dir_AirMove.x;
                dir_GroundMove.z = dir_AirMove.z;
            }

            dir_GroundMove.y -= force_Gravity * Time.deltaTime;
            // 对角色控制器进行移动操作并检测与地面是否接触
            is_Grounded = (controller.Move(dir_GroundMove * Time.deltaTime) & CollisionFlags.Below) != 0;


        }

        public bool CancelCrouch()
        {
            Vector3 bottom = transform.position - (Vector3.up * ((height_Crouch / 2) - info_Player.radius));
            // 强制物理判定头顶是否有障碍物,有则返回
            bool isBlocked = 
                Physics.SphereCast(
                    bottom, 
                    info_Player.radius, 
                    Vector3.up, 
                    out var hit, 
                    info_Player.height - info_Player.radius, 
                    PlayController.GetInstance().layer_Collsion);
            if (isBlocked)
                return false; 
            controller.height = info_Player.height;
            return true;
        }

        public bool InCrouching()
        {
            return (t_status == E_Move_Status.Crouching);
        }

        public void CheckCrouchAct()
        {
            if (!is_Grounded || (int)t_status > 2)
                return ;

            if (PlayController.GetInstance().input_IsSprint)
            {
                CancelCrouch();
                return ;
            }

            if (PlayController.GetInstance().input_IsCrouching)
            {
                if (t_status != E_Move_Status.Crouching)
                {
                    controller.height = height_Crouch;
                    PlayController.GetInstance().SwitchStatus(E_Move_Status.Crouching);
                }
                else
                    CancelCrouch();
            }
        }

        private bool InSprinting( )
        {
            return (t_status == E_Move_Status.Sprinting && is_Grounded);
        }

        private bool ReadyToSprinting()
        {
            bool sprint = 
                (PlayController.GetInstance().input_IsSprint && PlayController.GetInstance().input_HoriDir.y > 0);

            return sprint;
        }

    }

    public class PlayerIntractInfo
    {
        public float rayDistance;
        public float radius;
        public float height;
        public float halfradius;
        public float halfheight;
        public float crouchCamAdjust;

        public PlayerIntractInfo(float r, float h, float ch)
        {
            radius = r; height = h;
            halfradius = r / 2f; halfheight = h / 2f;
            rayDistance = halfheight + radius + .175f;
            crouchCamAdjust = (ch - height) / 2f;
        }
    }


}