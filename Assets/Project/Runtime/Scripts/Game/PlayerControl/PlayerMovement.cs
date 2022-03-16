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
        public float speed_Climb = 2f;
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

        public Vector3 Input_Jump { set => dir_Jump = value; }
        private Vector3 dir_Jump = Vector3.zero;



        // 跳跃计时器和判定
        public int Timer_Jump { set => timer_Jump = value; }
        private int timer_Jump;

        // 冲刺相关数据
        [SerializeField]
        private float time_Sprint = 6f;
        [SerializeField]
        private float reduce_Sprint = 3f;
        [SerializeField]
        private float reserve_Sprint = 4f;
        [SerializeField]
        private float minlesstime_Sprint = 1f;
        // 力竭判定
        private bool trigger_SprintLess; 
        // 冲刺耐力
        private float stamina;
        // 状态缓存
        private E_Move_Status t_status;

        // 蹲俯相关数据
        public float Height_Crouch { get => height_Crouch; }

        [SerializeField]
        private float height_Crouch = 1f;
        [SerializeField]
        private float height_Jump_idle = 1f;
        [SerializeField]
        private float height_Jump_Crouch = 0.5f;

        [HideInInspector]
        public CharacterController controller;
        [HideInInspector]
        public PlayerIntractInfo info_Player;

        // 特殊动作相关数据
        private List<SpecialMoveType> specialMovements;
        private bool trigger_SpecialMove;
        private bool trigger_EnforceSprintReserve;
        private bool trigger_EnforceGravity;
        private float time_Enforce = 0f;
        private float force_Jump = 0f;

        // Debug相关
        [SerializeField]
        private Mesh mesh_playWalking, mesh_playCrouch;
        [SerializeField]
        [Range(0f, 1f)] private float scale_Debug = 0.5f;


        private void Start()
        {
            timer_Jump = -1;
            trigger_EnforceGravity = false;
            trigger_SprintLess = false;
            controller = GetComponent<CharacterController>();
            info_Player = new PlayerIntractInfo(controller.radius, controller.height, height_Crouch);
            stamina = time_Sprint;

            SpecialMoveType sliding = GetComponent<SlidingMovement>();
            SpecialMoveType vaulting = GetComponent<VaultOverMovement>();
            SpecialMoveType labbering = GetComponent<LabberClimbMovement>();
            sliding.InitMovement();
            vaulting.InitMovement();
            labbering.InitMovement();
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

            ReadyToSpecialMove();
            UpdateMovingStatus();

            CheckCrouchAct();
            foreach (SpecialMoveType moveType in specialMovements)
            {
                if (moveType.enabled && trigger_SpecialMove)
                {
                    moveType.CheckSpecialAct();
                }
                    
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (time_Enforce > 0)
            {
                if (trigger_EnforceGravity)
                {
                    dir_GroundMove.y -= force_Gravity * Time.deltaTime;
                }
                    
                is_Grounded = (controller.Move(dir_GroundMove * Time.deltaTime) & CollisionFlags.Below) != 0;
            }

            // 特殊移动状态判定
            foreach (SpecialMoveType moveType in specialMovements)
            {
                if (t_status == moveType.changeTo)
                {
                    moveType.Movement();
                    return;
                }

            }

            //注入默认移动状态
            UpdateGlobalMovement();
        }

        private void UpdateGlobalMovement()
        {
            GenaralGroundMove();

            CheckJumpAct();
        }

        #region 跳跃动作相关
        private void CheckJumpAct()
        {
            // 更新跳跃
            bool jump = PlayController.GetInstance().input_IsJumping;

            if (!jump)
            {
                timer_Jump++;
                return;
            }
            else if (timer_Jump > 0 && is_Grounded)
            {
                if (t_status != E_Move_Status.Crouching)
                {
                    dir_Jump = Vector3.up * height_Jump_Crouch;
                    timer_Jump = -1;
                }
                else
                {
                    dir_Jump = Vector3.up * height_Jump_idle;
                    timer_Jump = -1;
                } 

            }
        }

        public void UpdateJump()
        {
            if(dir_Jump != Vector3.zero)
            {
                Vector3 dir = (dir_Jump * speed_Jump);
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
            dir_Jump = Vector3.zero;
        }

        #endregion

        #region 全局动作控制
        private void UpdateMovingStatus()
        {
            t_status = PlayController.GetInstance().status_Current;

            CaculateStanmina();

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
                UpdateJump();
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
        #endregion

        #region 蹲俯动作相关
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

            if (PlayController.GetInstance().input_IsSprint && InCrouching())
            {

                if (CancelCrouch())
                {
                    PlayController.GetInstance().SwitchStatus(E_Move_Status.Sprinting);
                }
                return;
            }
            else if (PlayController.GetInstance().input_IsJumping)
            {
                if (CancelCrouch())
                {
                    PlayController.GetInstance().SwitchStatus(E_Move_Status.Idle);
                }
                return;
            }

            if (PlayController.GetInstance().input_IsCrouching)
            {
                if (t_status != E_Move_Status.Crouching)
                {
                    controller.height = height_Crouch;
                    PlayController.GetInstance().SwitchStatus(E_Move_Status.Crouching);
                }
                else
                {
                    if(CancelCrouch())
                        PlayController.GetInstance().SwitchStatus(E_Move_Status.Walking);
                }

            }
        }
        #endregion

        #region 冲刺动作相关
        private bool InSprinting( )
        {
            return (t_status == E_Move_Status.Sprinting && is_Grounded);
        }

        public bool ReadyToSprinting()
        {

            bool sprint = 
                (PlayController.GetInstance().input_IsSprint && PlayController.GetInstance().input_HoriDir.y > 0 && !trigger_SprintLess);
            if (t_status != E_Move_Status.Sliding)
            {
                if (!InSprinting())
                {
                    if (trigger_EnforceSprintReserve && stamina < reserve_Sprint)
                        return false;
                    else if (!trigger_EnforceSprintReserve && stamina < minlesstime_Sprint)
                        return false;
                }
                if (stamina <= 0)
                {
                    trigger_EnforceSprintReserve = true;
                    return false;
                }
            }
            if (sprint)
                trigger_EnforceSprintReserve = false;
            return sprint;
        }
        

        private void CaculateStanmina()
        {

            if (t_status == E_Move_Status.Sprinting && stamina > 0 && !trigger_SprintLess)
            {
                stamina -= Time.deltaTime * reduce_Sprint;
                if (stamina < 0)
                {
                    stamina = 0;
                    trigger_SprintLess = true; 
                }
                
            }
            else if (stamina < time_Sprint && !PlayController.GetInstance().input_IsSprint)
            {
                stamina += Time.deltaTime * reduce_Sprint;
            }

            if (stamina >= minlesstime_Sprint)
            {
                    trigger_SprintLess = false;
            }

            EventCenter.GetInstance().EventTrigger("SetStaminaCurrent", Mathf.Clamp( stamina / time_Sprint  , 0f, 1f));

            EventCenter.GetInstance().EventTrigger("SetStaminaMinless", Mathf.Clamp( minlesstime_Sprint / time_Sprint, 0f, 1f));
        }

        #endregion

        #region 特殊动作相关

        // 进入特殊运动状态判定
        private void ReadyToSpecialMove()
        {
            //已处于特殊动作状态
            if ((int)t_status >= 5)
                trigger_SpecialMove = false;
            else if (!trigger_SpecialMove)
            {
                if (is_Grounded || dir_GroundMove.y < 0)
                {
                    trigger_SpecialMove = true;
                }
                    
            }
            
        }

        // 添加特殊动作队列
        public void AddSpeicalMoveType( SpecialMoveType movetype)
        {
            if (specialMovements == null)
                specialMovements = new List<SpecialMoveType>();


            specialMovements.Add(movetype);

        }

        public void SpecialMove(Vector3 dirsp, float speed, float appliedGravity, float setY = 0)
        {
            if (time_Enforce > 0)
                return;

            Vector3 move = dirsp * speed;
            if (appliedGravity > 0)
            {
                dir_GroundMove.x = move.x;
                if (setY != 0)
                    dir_GroundMove.y = setY * speed;
                dir_GroundMove.y -= force_Gravity * Time.deltaTime * appliedGravity;
                dir_GroundMove.z = move.z;
            }
            else
                dir_GroundMove = move;

            UpdateJump();

            is_Grounded = (controller.Move(dir_GroundMove * Time.deltaTime) & CollisionFlags.Below) != 0;

        }


        public void SetEnforceMove(Vector3 dir, float time, bool isEnforcegravity)
        {
            time_Enforce = time;
            trigger_EnforceGravity = isEnforcegravity;
            dir_GroundMove = dir * speed_Walk;
        }
        #endregion

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            if (info_Player != null)
            {
                Vector3 bottom = transform.position - (Vector3.up * (info_Player.height / 2));
                if (!InCrouching())
                {
                    Gizmos.DrawWireMesh(mesh_playWalking,
                        bottom,
                        transform.rotation,
                        Vector3.one * scale_Debug);
                }
                else
                {
                    bottom = transform.position - (Vector3.up * ((height_Crouch / 2) - info_Player.radius));
                    Gizmos.DrawWireMesh(mesh_playCrouch,
                    bottom,
                    transform.rotation,
                    Vector3.one * scale_Debug);
                }

            }


            

        }

        #endif

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