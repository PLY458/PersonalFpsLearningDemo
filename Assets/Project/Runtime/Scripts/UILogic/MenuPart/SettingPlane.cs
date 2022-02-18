using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPlane : UIBasePanel
{
    private Image img_Curtain;
    private Image img_Frame;

    private CanvasGroup _curtainAlpha;

    private void OnEnable()
    {
        _curtainAlpha.alpha = 0;
        _curtainAlpha.LeanAlpha(1, 0.6f);

        img_Frame.transform.localPosition = new Vector2(-Screen.width, 0);
        img_Frame.transform.LeanMoveLocalX(0, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    protected override void InitPanel()
    {
        base.InitPanel();

        img_Curtain = GetControl<Image>("img_Curtain");
        img_Frame = GetControl<Image>("img_Frame");

        _curtainAlpha = img_Curtain.gameObject.GetComponent<CanvasGroup>();

        EventCenter.GetInstance().AddEventListener<bool>("Display_SettingPlane", DisplayPlane);
    }

    private void ExitDialog()
    {
        _curtainAlpha.LeanAlpha(0, 0.6f);
        img_Frame.transform.LeanMoveLocalX(-Screen.width, 0.5f)
            .setEaseInExpo().setOnComplete(() => gameObject.SetActive(false));
    }
}
