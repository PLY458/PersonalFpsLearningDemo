using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS_Movement_Control
{

    [DisallowMultipleComponent]
    public class LabberClimbMovement : SpecialMoveType
    {
        [SerializeField]
        private LayerMask layer_Labber;
        Vector3 normel_Labber = Vector3.zero;
        Vector3 touch_Last = Vector3.zero;

        Vector3 t_input = Vector2.zero;

        public override void Movement()
        {
            t_input = PlayController.GetInstance().input_HoriDir;
            Vector3 move = Vector3.Cross(Vector3.up, normel_Labber).normalized;
            move *= t_input.x;
            move.y = t_input.y * playmovement.speed_Walk;

            // 下梯子判定
            bool goToGround = (move.y < -0.02f && playmovement.Is_Grounded);

            // 先进行跳跃判定
            if (PlayController.GetInstance().input_IsJumping)
            {
                playmovement.Input_Jump = (-normel_Labber + Vector3.up * 2f).normalized * 1f;
                playmovement.Timer_Jump = -1;
                PlayController.GetInstance().SwitchStatus(E_Move_Status.Walking);
            }

            // 下梯子实行强行移动
            if (!HasObjectInfront(0.05f) || goToGround)
            {
                PlayController.GetInstance().SwitchStatus(E_Move_Status.Walking);
                Vector3 pushUp = normel_Labber;
                pushUp.y = 0.25f;

                playmovement.SetEnforceMove(pushUp, 0.25f, true);
            }
            else
                playmovement.SpecialMove(move, 1f, 0f);
        }

        public override void CheckSpecialAct()
        {
           
            // 检查前方是否处于梯子物体上
            bool right = Physics.Raycast(transform.position + (transform.right * playmovement.info_Player.halfradius), 
                transform.forward, playmovement.info_Player.radius + 0.125f, layer_Labber);

            if (!right)
            Debug.DrawRay(transform.position + (transform.right * playmovement.info_Player.halfradius),
                transform.forward, Color.red);

            bool left = Physics.Raycast(transform.position - (transform.right * playmovement.info_Player.halfradius),
                transform.forward, playmovement.info_Player.radius + 0.125f, layer_Labber);

            if(!left)
            Debug.DrawRay(transform.position - (transform.right * playmovement.info_Player.halfradius),
                transform.forward, Color.blue);

            if (Physics.Raycast(transform.position, transform.forward, out var hit, playmovement.info_Player.radius + 0.125f, layer_Labber) && right && left)
            {
                // 保证正面朝向梯子物体
                if (hit.normal != hit.transform.forward)
                    return;

                normel_Labber = -hit.normal;

                if (HasObjectInfront(0.1f) && PlayController.GetInstance().input_HoriDir.y > 0.02f)
                {  
                    PlayController.GetInstance().SwitchStatus(changeTo);
                }
                    
            }
        }

        private bool HasObjectInfront(float dis)
        {
            Vector3 top = transform.position + (transform.forward * 0.25f);
            Vector3 bottom = top - (transform.up * playmovement.info_Player.halfheight);

            bool moreThanTwo = (Physics.CapsuleCastAll(top, bottom, 0.25f, transform.forward, dis, layer_Labber).Length >= 1);
            if(moreThanTwo)
                Debug.DrawRay(transform.position, transform.forward, Color.yellow);
            return moreThanTwo;
        }
    }
}

