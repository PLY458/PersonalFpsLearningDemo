using System;
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
        Sprinting,
        Sliding,
        ClimbingLadder,
        Vaulting,
        GrabbedLedge,
        ClimbingLedge
    }


    public class PlayController : SingletonMono<PlayController>
    {
        // �˶���ײ�㼶����
        public LayerMask layer_moveCollsion;

        // ��ǰ�˶�״̬
        public E_Move_Status move_Current;

        // �����˶����������
        [HideInInspector]
        public Vector3 input_HoriDir;
        [HideInInspector]
        public Vector2 input_RawDir;
        [HideInInspector]
        public bool input_IsSprint;
        [HideInInspector]
        public bool input_SprintCancel;
        [HideInInspector]
        public bool input_IsCrouching;
        [HideInInspector]
        public bool input_IsJumping;
        [HideInInspector]
        public Vector2 input_MouseView;
        [HideInInspector]
        public bool input_IsInteract;

        private bool hidingCursor;

        public PlayerMovement movement_Player;
        public CameraMovement movement_Camera;

        public AnimateCameraLevel animateCamLevel;
        public AnimateLean animateLean;

        Vector2 previous;
        Vector2 _down;

        public Vector2 Down { get => _down; }

        void Start()
        {
            hidingCursor = true;
            
            // ע����������Ӧ���¼�
            EventCenter.GetInstance().AddEventListener<Vector3>("GetMoveDirInput", (input) => input_HoriDir = input);
            EventCenter.GetInstance().AddEventListener<Vector2>("GetRawDirInput", (input) => input_RawDir = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetMoveRunInput", (input) => input_IsSprint = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetMoveRunCancel", (input) => input_SprintCancel = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetCrouchInput", (input) => input_IsCrouching = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetJumpInput", (input) => input_IsJumping = input);
            EventCenter.GetInstance().AddEventListener<Vector2>("GetViewInput", (input) => input_MouseView = input);
            EventCenter.GetInstance().AddEventListener<bool>("ControlPlayMenu", PlayMenuControl);
            EventCenter.GetInstance().AddEventListener<bool>("GetInteractInput", (input) => input_IsInteract = input);

            PlayerInput.GetInstance().InitInput();
            PlayerInput.GetInstance().StartOrEndCheck(true);

        }

        void Update()
        {
            UpdateLean();
            UpdateCamLevel();
            UpdateLedgeDown();
        }



        #region ״̬�������

        public void SwitchStatus(E_Move_Status stu)
        {
            if (move_Current == stu)
                return;

            move_Current = stu;
        }

        public void CursorControl()
        {
            Cursor.visible = hidingCursor;
            if (!hidingCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                EventCenter.GetInstance().EventTrigger("Undisplay_RankListPlane");
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                EventCenter.GetInstance().EventTrigger("Display_RankListPlane", true);

            }

            hidingCursor = !hidingCursor;
        }

        private void PlayMenuControl(bool input)
        {
            if (!input)
                return;
            else
            {
                CursorControl();
            }
            

        }

        private void UpdateLedgeDown()
        {
            Vector2 raw = input_RawDir;

            _down = Vector2.zero;
            if (raw.x != previous.x)
            {
                previous.x = raw.x;
                if (previous.x != 0)
                    _down.x = previous.x;
            }
            if (raw.y != previous.y)
            {
                previous.y = raw.y;
                if (previous.y != 0)
                    _down.y = previous.y;
            }

        }

        #endregion

        #region ���������й�


        private void UpdateCamLevel()
        {
            if (animateCamLevel == null) return;

            float level = 0f;
            if (move_Current == E_Move_Status.Crouching || move_Current == E_Move_Status.Sliding || move_Current == E_Move_Status.Vaulting || move_Current == E_Move_Status.ClimbingLadder )
                level = movement_Player.info_Player.crouchCamAdjust;
            animateCamLevel.UpdateLevel(level);
        }

        private void UpdateLean()
        {
            if (animateLean == null)
                return;
            Vector2 lean = Vector2.zero;
            if (move_Current == E_Move_Status.Sliding)
                lean.y = -1;
            else if (move_Current == E_Move_Status.Vaulting)
                lean.y = 1;
            animateLean.SetLean(lean);
        }

        #endregion

    }

}


