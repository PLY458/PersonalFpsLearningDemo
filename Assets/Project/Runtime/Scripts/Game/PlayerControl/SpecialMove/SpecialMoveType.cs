using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS_Movement_Control
{
    public class SpecialMoveType : MonoBehaviour
    {
        [SerializeField]
        public E_Move_Status changeTo;

        protected E_Move_Status currentStatus;

        protected PlayerMovement playmovement;


        public virtual void InitMovement()
        {
            playmovement = PlayController.GetInstance().movement_Player;
            playmovement.AddSpeicalMoveType(this);

        }

        // 特殊动作的运动方法
        public virtual void Movement()
        {

        }

        // 特殊动作各种状态的判定
        public virtual void CheckSpecialAct()
        {

        }


    }
}

