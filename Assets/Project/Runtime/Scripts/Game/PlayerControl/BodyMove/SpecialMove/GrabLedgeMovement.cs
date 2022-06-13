using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS_Movement_Control
{

    [DisallowMultipleComponent]
    public class GrabLedgeMovement : SpecialMoveType
    {
        [SerializeField]
        private LayerMask Layer_Ledge;
        [HideInInspector]
        public Vector3 pushFrom;
        [HideInInspector]
        public bool canGrabLedge = true;

        Vector3 t_input = Vector2.zero;

        public override void Movement()
        {

            t_input = PlayController.GetInstance().input_RawDir;
            if (PlayController.GetInstance().input_IsJumping && t_input.y <= 0)
            {
                playmovement.Input_Jump = (Vector3.up - transform.forward).normalized * 1f;
                playmovement.Timer_Jump = -1;
                PlayController.GetInstance().SwitchStatus(E_Move_Status.Walking);
            }

            playmovement.SpecialMove(Vector3.zero, 0f, 0f); //Stay in place
        }

        public override void CheckSpecialAct()
        {
            if (PlayController.GetInstance().move_Current == changeTo)
            {
                canGrabLedge = false;
                if (PlayController.GetInstance().Down.y == -1)
                    PlayController.GetInstance().SwitchStatus(E_Move_Status.Walking);
            }

            if (playmovement.Is_Grounded || playmovement.dir_GroundMove.y > 0)
                canGrabLedge = true;

            if (!canGrabLedge) return;

            if (!playmovement.Is_Grounded && playmovement.dir_GroundMove.y >= 0)
                return;

            //Check for ledge to grab onto 
            Vector3 dir = transform.TransformDirection(new Vector3(0, -0.5f, 1).normalized);
            Vector3 pos = transform.position + (Vector3.up * playmovement.info_Player.height / 3f) + (transform.forward * playmovement.info_Player.radius / 2f);
            bool right = Physics.Raycast(pos + (transform.right * playmovement.info_Player.radius / 2f), dir, playmovement.info_Player.radius + 0.125f, Layer_Ledge);
            bool left = Physics.Raycast(pos - (transform.right * playmovement.info_Player.radius / 2f), dir, playmovement.info_Player.radius + 0.125f, Layer_Ledge);

            if (Physics.Raycast(pos, dir, out var hit, playmovement.info_Player.radius + 0.125f, Layer_Ledge) && right && left)
            {
                Vector3 rotatePos = transform.InverseTransformPoint(hit.point);
                rotatePos.x = 0; rotatePos.z = 1;
                pushFrom = transform.position + transform.TransformDirection(rotatePos); //grab the position with local z = 1
                rotatePos.z = playmovement.info_Player.radius * 2f;

                Vector3 checkCollisions = transform.position + transform.TransformDirection(rotatePos); //grab it closer now

                //Check if you would be able to stand on the ledge
                if (!Physics.SphereCast(checkCollisions, playmovement.info_Player.radius, Vector3.up, out hit, playmovement.info_Player.height - playmovement.info_Player.radius))
                    PlayController.GetInstance().SwitchStatus(changeTo);
            }
        }
    }

}

