using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ExitPlane : UIBasePanel
{
    private Image img_Curtain;
    private Image img_Frame;

    private Button btn_Cancel;
    private Button btn_Exit;

    private CanvasGroup _curtainAlpha;

    private void OnEnable()
    {
        _curtainAlpha.alpha = 0;
        _curtainAlpha.LeanAlpha(1, 0.6f);

        img_Frame.transform.localPosition = new Vector2(0, -Screen.height );
        img_Frame.transform.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    protected override void InitPanel()
    {
        base.InitPanel();

        img_Curtain = GetControl<Image>("img_Curtain");
        img_Frame = GetControl<Image>("img_Frame");
        btn_Cancel = GetControl<Button>("btn_Cancel");
        btn_Exit = GetControl<Button>("btn_Exit");

        _curtainAlpha =  img_Curtain.gameObject.GetComponent<CanvasGroup>();

        btn_Cancel.onClick.AddListener(ExitDialog);

        EventCenter.GetInstance().AddEventListener<bool>("Display_ExitPlane", DisplayPlane);

        DisplayPlane(false);
    }


    private void ExitDialog()
    {
        _curtainAlpha.LeanAlpha(0, 0.6f);
        img_Frame.transform.LeanMoveLocalY(-Screen.height * 1.5f, 0.5f)
            .setEaseInExpo().setOnComplete(()=> gameObject.SetActive(false));
    }



}
