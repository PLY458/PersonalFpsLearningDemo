using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS_Movement_Control
{
    [DisallowMultipleComponent]
    public class VaultOverMovement : SpecialMoveType
    {
        [SerializeField]
        private LayerMask layer_Vault;

        Vector3 over_Vault;
        Vector3 dir_Vault;

        GameObject vaultHelper;

        void CreateVaultHelper()
        {
            vaultHelper = new GameObject();
            vaultHelper.transform.name = "_Vault Helper";
        }

        void SetVaultHelper()
        {
            vaultHelper.transform.position = over_Vault;
            vaultHelper.transform.rotation = Quaternion.LookRotation(dir_Vault);
        }

        public override void InitMovement()
        {
            base.InitMovement();
            CreateVaultHelper();
        }

        public override void Movement()
        {
            Vector3 dir = over_Vault - transform.position;
            Vector3 localPos = vaultHelper.transform.InverseTransformPoint(transform.position);
            Vector3 move = (dir_Vault + (Vector3.up * -(localPos.z - playmovement.info_Player.radius) * playmovement.info_Player.height)).normalized;

            if (localPos.z < -(playmovement.info_Player.radius * 2f))
                move = dir.normalized;
            else if (localPos.z > playmovement.info_Player.halfheight)
            {
                playmovement.controller.height = playmovement.info_Player.height;
                PlayController.GetInstance().SwitchStatus(E_Move_Status.Walking);
            }

            playmovement.SpecialMove(move, playmovement.speed_Sprint, 0f);
        }

        public override void CheckSpecialAct()
        {
            currentStatus = PlayController.GetInstance().status_Current;

            if (currentStatus == changeTo)
                return;

            float movementAdjust = (Vector3.ClampMagnitude(playmovement.controller.velocity, 16f).magnitude / 16f);
            float checkDis = playmovement.info_Player.radius + movementAdjust;

            if (HasObjectInfront(checkDis) && PlayController.GetInstance().input_IsJumping)
            {
                if (Physics.SphereCast(transform.position + (transform.forward * (playmovement.info_Player.radius - 0.25f)), 0.25f, transform.forward, out var sphereHit, checkDis, layer_Vault))
                {
                    if (Physics.SphereCast(sphereHit.point + (Vector3.up * playmovement.info_Player.halfheight), 
                        playmovement.info_Player.radius, Vector3.down, out var hit, playmovement.info_Player.halfheight - playmovement.info_Player.radius, layer_Vault))
                    {
                        Debug.DrawRay(hit.point + (Vector3.up * playmovement.info_Player.radius), Vector3.up * playmovement.info_Player.halfheight);
                        //Check above the point to make sure the player can fit
                        if (Physics.SphereCast(hit.point + (Vector3.up * playmovement.info_Player.radius), playmovement.info_Player.radius, Vector3.up, out var trash, playmovement.info_Player.halfheight))
                            return; //If cannot fit the player then do not vault

                        //Check in-front of the vault to see if something is blocking
                        Vector3 fromPlayer = transform.position;
                        Vector3 toVault = hit.point + (Vector3.up * playmovement.info_Player.radius);
                        fromPlayer.y = toVault.y;

                        Vector3 dir = (toVault - fromPlayer);
                        if (Physics.SphereCast(fromPlayer, playmovement.info_Player.radius / 2f, dir.normalized, out var trash2, dir.magnitude + playmovement.info_Player.radius))
                            return;
                        //If we hit something blocking the vault, then do nothing

                        over_Vault = hit.point;
                        dir_Vault = transform.forward;
                        SetVaultHelper();

                        playmovement.controller.height = playmovement.info_Player.radius;
                        PlayController.GetInstance().SwitchStatus(changeTo);
                    }
                }
            }
        }

        private bool HasObjectInfront(float dis)
        {
            Vector3 top = transform.position + (transform.forward * 0.25f);
            Vector3 bottom = top - (transform.up * playmovement.info_Player.halfheight);

            return (Physics.CapsuleCastAll(top, bottom, 0.25f, transform.forward, dis, layer_Vault).Length >= 1);
        }

    }
}
