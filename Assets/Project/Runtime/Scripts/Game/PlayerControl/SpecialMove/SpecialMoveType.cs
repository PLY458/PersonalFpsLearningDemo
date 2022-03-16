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

        // ���⶯�����˶�����
        public virtual void Movement()
        {

        }

        // ���⶯������״̬���ж�
        public virtual void CheckSpecialAct()
        {

        }


    }
}

