using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS_Movement_Control
{
    [DisallowMultipleComponent]
    public class SlidingMovement : SpecialMoveType
    {
        [SerializeField]
        [Range(6.0f, 9.0f)] private float speedMin_Slide = 8.0f;
        [SerializeField]
        [Range(9.0f, 12.0f)] private float speedMax_Slide = 10.0f;

        private Vector3 pos_Ground, dir_slide;

        private bool controlledSlide;

        private float slideTime , slideLimit;
        private float slideBlendTime = 0.222f;
        private float slideDownward = 0f;

        

        public override void InitMovement()
        {
            base.InitMovement();
            slideLimit = playmovement.controller.slopeLimit - .1f;

        }

        public override void Movement()
        {
            if (playmovement.Is_Grounded && PlayController.GetInstance().input_IsJumping)
            {
                if (controlledSlide)
                    dir_slide = transform.forward;
                playmovement.Input_Jump = (dir_slide + Vector3.up) * 1f;
                playmovement.Timer_Jump = -1;
                slideTime = 0;
            }

            float blend = Mathf.Clamp(slideTime, 0f, slideBlendTime) / slideBlendTime;
            float speed = Mathf.Lerp(speedMin_Slide, speedMax_Slide, slideDownward);
            playmovement.SpecialMove(dir_slide, speed*blend, 1f, dir_slide.y);

        }

        public override void CheckSpecialAct()
        {

            // 检测玩家位置与地面的角度是否合适
            if (Physics.Raycast(transform.position, -Vector3.up, out var hit, 
                playmovement.info_Player.rayDistance, 
                PlayController.GetInstance().layer_Collsion)) 
            {
                currentStatus = PlayController.GetInstance().status_Current;

                float angle = Vector3.Angle(hit.normal, Vector3.up);
                Vector3 hitNormal = hit.normal;

                Vector3 slopeDir = Vector3.ClampMagnitude(new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z), 1f);
                Vector3.OrthoNormalize(ref hitNormal, ref slopeDir);


                if (angle > 0 && currentStatus == changeTo) //Adjust to slope direction
                {
                    Debug.DrawRay(transform.position - Vector3.up * playmovement.info_Player.halfheight, dir_slide, Color.green);
                    dir_slide = Vector3.RotateTowards(dir_slide, slopeDir, speedMin_Slide * Time.deltaTime / 2f, 0.0f);
                }
                else
                    dir_slide.y = 0;

                // 
                if (angle > slideLimit && currentStatus != changeTo)
                {
                    playmovement.controller.height = playmovement.Height_Crouch;
                    // 先转换到蹲俯状态
                    PlayController.GetInstance().SwitchStatus(E_Move_Status.Crouching);
                    dir_slide = slopeDir;
                    controlledSlide = false;
                    slideTime = slideBlendTime;
                    // 再转换到目标状态
                    PlayController.GetInstance().SwitchStatus(changeTo);
                }
                else if (currentStatus == changeTo)
                {
                    dir_slide.y = 0;
                    dir_slide = dir_slide.normalized;
                    slideDownward = 0f;
                }

                //Check to slide when running
                if (ReadySlideInRunning())
                {
                    PlayController.GetInstance().SwitchStatus(changeTo);
                    dir_slide = transform.forward;
                    playmovement.controller.height = playmovement.Height_Crouch;
                    controlledSlide = true;
                    slideDownward = 0f;
                    slideTime = 1f;
                }

                //Lower slidetime
                if (slideTime > 0)
                {
                    if (dir_slide.y < 0)
                    {
                        slideDownward = Mathf.Clamp(slideDownward + Time.deltaTime * Mathf.Sqrt(Mathf.Abs(dir_slide.y)), 0f, 1f);
                        if (slideTime <= slideBlendTime)
                            slideTime += Time.deltaTime;
                    }
                    else
                    {
                        slideDownward = Mathf.Clamp(slideDownward - Time.deltaTime, 0f, 1f);
                        slideTime -= Time.deltaTime;
                    }

                    if (controlledSlide && slideTime <= slideBlendTime)
                    {
                        if (playmovement.ReadyToSprinting() && playmovement.CancelCrouch())
                            PlayController.GetInstance().SwitchStatus(E_Move_Status.Sprinting);
                    }

  
                }
                else if (currentStatus == changeTo)
                {


                    if (PlayController.GetInstance().input_IsCrouching)
                    {
                        playmovement.controller.height = playmovement.Height_Crouch;
                        PlayController.GetInstance().SwitchStatus(E_Move_Status.Crouching);
                    }
                    else if (!playmovement.CancelCrouch()) // 尝试取消
                    {
                        playmovement.controller.height = playmovement.Height_Crouch;
                        PlayController.GetInstance().SwitchStatus(E_Move_Status.Crouching);
                    }
                    else
                    {
                        playmovement.controller.height = playmovement.info_Player.height;
                        PlayController.GetInstance().SwitchStatus(E_Move_Status.Idle);
                    }

                }



            }

        }

        private bool ReadySlideInRunning()
        {
            if (!PlayController.GetInstance().input_IsCrouching || !PlayController.GetInstance().input_IsSprint)
                return false;

            if (slideTime > 0 || currentStatus == changeTo)
                return false;

            return true;

        }
    }

}
