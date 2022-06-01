using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS_Movement_Control;

public class InteractMgr : SingletonMono<InteractMgr>
{
    // Ωªª•ºÏ≤‚≤„º∂…Ë÷√
    public LayerMask layer_interact;
    // Ωªª•æ‡¿Î
    [Range(0.5f, 3)] public float distance_interact;

    private Transform mainCamTrans;

    private InteractObject tempObj;

    private void Start()
    {
        mainCamTrans = PlayController.GetInstance().movement_Camera.transform;
    }

    private void Update()
    {
        if (IsInteractable())
        {
            EventCenter.GetInstance().EventTrigger("Display_InteractTip", tempObj.description);
            if(PlayController.GetInstance().input_IsInteract)
                tempObj.Interact();
        }
        else
        {
            EventCenter.GetInstance().EventTrigger("Display_InteractTip", "");
        }

    }

    private bool IsInteractable()
    {
        tempObj = null;

        Ray tempRay = new Ray(mainCamTrans.position, mainCamTrans.forward);
        if (Physics.Raycast(tempRay, out var hit, 5f, PlayController.GetInstance().layer_moveCollsion))
        {
            float dis = Vector3.Distance(mainCamTrans.position, hit.point) + 0.05f;
            if (Physics.Raycast(tempRay, out var interact, dis, layer_interact))
            {
                InteractObject infront = interact.transform.GetComponent<InteractObject>();
                if (infront == null)
                    return false;
                if (dis > infront.interactRange + 0.05f)
                    infront = null;
                tempObj = infront; //Set interactWith to the one we hit

                if (tempObj != null && tempObj.isEnableInteract)
                {
                    return true;
                }
            }
        }

        return false;

    }

}
