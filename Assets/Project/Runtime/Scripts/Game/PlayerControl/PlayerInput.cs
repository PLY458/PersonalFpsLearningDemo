using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class PlayerInput : SingletonAutoMono<PlayerInput>
    {
        /// <summary>
        /// 开启检测开关
        /// </summary>
        private bool isStart = false;

        private PlayerInputData inputData;

        // 输入数据字典
        private Dictionary<string, object> inputDic = new Dictionary<string, object>();

        public void InitInput()
        {
            inputData = new PlayerInputData();
        }

        private void CheckValueInput<T>( T inputValue, string methodname = "")
        {
            EventCenter.GetInstance().EventTrigger(methodname, inputValue);
        }


        /// <summary>
        /// 是否开启输入检测
        /// </summary>
        public void StartOrEndCheck(bool isOpen)
        {
            isStart = isOpen;
        }

        public void Update()
        {
            if (!isStart)
            {
                return;
            }
            // 检查值传递即是监听输入
            // 运动相关:
            CheckValueInput(inputData.MoveDirInput, "GetMoveDirInput");
            //CheckValueInput(inputData.MouseViewInput, "");
            CheckValueInput(inputData.MoveRunKeeper, "GetMoveRunInput");

            CheckValueInput(inputData.MoveRunUp, "GetMoveRunCancel");

            CheckValueInput(inputData.CrouchTrigger, "GetCrouchInput");
            //CheckValueInput(inputData.CrouchKeeper, "");
            CheckValueInput(inputData.JumpKeeper, "GetJumpInput");

            CheckValueInput(inputData.CollectMenuTrigger, "ControlPlayMenu");

            CheckValueInput(inputData.MouseViewInput, "GetViewInput");

            CheckValueInput(inputData.InteractTrigger, "GetInteractInput");

            // 武器相关：
            CheckValueInput(inputData.ShootTrigger, "GetFireInput");

            CheckValueInput(inputData.AimTrigger, "GetAimInput");

            CheckValueInput(inputData.ReloadTrigger, "GetReloadInput");
        }

    }


    /// <summary>
    /// 玩家数据类
    /// </summary>
    public class PlayerInputData
    {

        /// <summary>
        /// 移动方向的输入
        /// </summary>
        public Vector3 MoveDirInput
        {
            get
            {
                Vector3 result = Vector3.zero;
                result.x = Input.GetAxis("Horizontal");
                result.y = Input.GetAxis("Vertical");
                result *= (result.x != 0.0f && result.y != 0.0f) ? .7071f : 1.0f;
                return result;
            }
        }

        public Vector2 MouseViewInput
        {
            get
            {
                Vector2 result = Vector2.zero;
                result.x = Input.GetAxisRaw("Mouse X");
                result.y = Input.GetAxisRaw("Mouse Y");
                return result;
            }
        }

        public bool CollectMenuTrigger
        {
            get { return Input.GetKeyDown(KeyCode.M); }
        }

        // 运动属性相关

        public bool MoveRunKeeper
        {
            get { return Input.GetKey(KeyCode.LeftShift); }
        }

        public bool MoveRunUp
        {
            get { return Input.GetKeyUp(KeyCode.LeftShift); }
        }

        public bool CrouchTrigger
        {
            get { return Input.GetKeyDown(KeyCode.C); }
        }

        public bool CrouchKeeper
        {
            get { return Input.GetKey(KeyCode.C); }
        }

        public bool JumpKeeper
        {
            get { return Input.GetKey(KeyCode.Space); }
        }   

        public bool InteractTrigger
        {
            get { return Input.GetKeyDown(KeyCode.F); }
        }
        // 射击属性相关
        public bool ReloadTrigger
        {
            get { return Input.GetKeyDown(KeyCode.R); }
        }

        public bool AimTrigger
        {
            get { return Input.GetMouseButtonDown(1); }
        }

        public bool AimKeeper
        {
            get { return Input.GetMouseButton(1); }
        }

        public bool ShootTrigger
        {
            get { return Input.GetMouseButton(0); }
        }

        public float MouseScroll
        {
            get { return Input.GetAxisRaw("Mouse ScrollWheel"); }
        }
    }


