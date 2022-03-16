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
        Vaulting

    }


    public class PlayController : SingletonMono<PlayController>
    {
        // ��ײ�㼶����
        public LayerMask layer_Collsion;

        // ��ǰ�˶�״̬
        public E_Move_Status status_Current;

        // �����˶����������
        [HideInInspector]
        public Vector3 input_HoriDir;
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

        private bool hidingCursor;


        public PlayerMovement movement_Player;
        public CameraMovement movement_Camera;

        void Start()
        {
            hidingCursor = true;
            
            // ע����������Ӧ���¼�
            EventCenter.GetInstance().AddEventListener<Vector3>("GetMoveDirInput", (input) => input_HoriDir = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetMoveRunInput", (input) => input_IsSprint = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetMoveRunCancel", (input) => input_SprintCancel = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetCrouchInput", (input) => input_IsCrouching = input);
            EventCenter.GetInstance().AddEventListener<bool>("GetJumpInput", (input) => input_IsJumping = input);
            EventCenter.GetInstance().AddEventListener<Vector2>("GetViewInput", (input) => input_MouseView = input);
            EventCenter.GetInstance().AddEventListener<bool>("ControlPlayMenu", PlayMenuControl);

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


        public void CursorControl()
        {
            Cursor.visible = hidingCursor;
            if (!hidingCursor)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;

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

        // Update is called once per frame
        void Update()
        {

        }


        #endregion

        #region ״̬�ж����


        #endregion


    }

}


