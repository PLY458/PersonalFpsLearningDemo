using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS_Movement_Control
{

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
            Debug.Log("开始输入检测！！！"+inputData.MoveDirInput);
            // 检查值传递即是监听输入
            CheckValueInput(inputData.MoveDirInput, "GetMoveDirInput");
            //CheckValueInput(inputData.MouseViewInput, "");
            CheckValueInput(inputData.MoveRunKeeper, "GetMoveRunInput");
            CheckValueInput(inputData.CrouchTrigger, "GetCrouchInput");
            //CheckValueInput(inputData.CrouchKeeper, "");
            CheckValueInput(inputData.JumpKeeper, "GetJumpInput");
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


        public bool MoveRunKeeper
        {
            get { return Input.GetKey(KeyCode.LeftShift); }
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
    }

}

