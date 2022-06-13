using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS_Movement_Control
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GrabLedgeMovement))]
    public class ClimbLedgeMovement : SpecialMoveType
    {
        GrabLedgeMovement grabLedge;

        public override void InitMovement()
        {
            base.InitMovement();
            grabLedge = GetComponent<GrabLedgeMovement>();
        }

        public override void Movement()
        {
            if (grabLedge == null) return;
            Vector3 dir = grabLedge.pushFrom - transform.position;
            Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
            Vector3 move = Vector3.Cross(dir, right).normalized;

            playmovement.Timer_Jump = -1;
            playmovement.SpecialMove(move, playmovement.speed_Sprint, 0f);
            if (new Vector2(dir.x, dir.z).magnitude < 0.125f)
                PlayController.GetInstance().SwitchStatus(E_Move_Status.Walking);
        }

        public override void CheckSpecialAct()
        {
            if (grabLedge == null)
                return;
            if (PlayController.GetInstance().move_Current == grabLedge.changeTo)
            {
                
                Vector2 dir = PlayController.GetInstance().input_RawDir;
                Debug.Log("½øÈëÅÊÅÀ¼ì²â: " + dir.ToString());

                if ( PlayController.GetInstance().Down.y == 1 || (PlayController.GetInstance().input_IsJumping && dir.y > 0))
                    PlayController.GetInstance().SwitchStatus(changeTo);
            }
        }
    }

}

