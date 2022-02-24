using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPlane : UIBasePanel
{
    private Image img_Curtain;
    private Image img_Frame;
    private Image img_TogBK;
    private Image img_Handle;

    private Button btn_Reset;
    private Button btn_Return;

    private Toggle tog_Slience;

    private CanvasGroup _curtainAlpha;
    private CanvasGroup _togBKAlpha;

    [SerializeField]
    private Sprite spr_hand_ON, spr_hand_OFF;

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
        img_TogBK = GetControl<Image>("img_TogBK");
        img_Handle = GetControl<Image>("img_Handle");

        btn_Return = GetControl<Button>("btn_Return");

        tog_Slience = GetControl<Toggle>("tog_Slience");

        _curtainAlpha = img_Curtain.gameObject.GetComponent<CanvasGroup>();
        _togBKAlpha = img_TogBK.gameObject.GetComponent<CanvasGroup>();

        btn_Return.onClick.AddListener(ExitDialog);

        tog_Slience.onValueChanged.AddListener(SwitchToggle);

        EventCenter.GetInstance().AddEventListener<bool>("Display_SettingPlane", DisplayPlane);

        DisplayPlane(false);
    }

    private void ExitDialog()
    {
        _curtainAlpha.LeanAlpha(0, 0.6f);
        img_Frame.transform.LeanMoveLocalX(-Screen.width, 0.5f)
            .setEaseInExpo().setOnComplete(() => gameObject.SetActive(false));
    }

    private void SwitchToggle(bool tog)
    {
        float dir = tog ? 44f : -44f;

        if (tog)
        {
            _togBKAlpha.LeanAlpha(1, 0.5f).setEaseInOutSine();
        }
        else
        {
            _togBKAlpha.LeanAlpha(0.1f, 0.5f).setEaseInOutSine();
        }

        img_Handle.transform.LeanMoveLocalX(dir, 0.5f)
            .setEaseInOutSine().setOnComplete(()=> 
            {
                if (tog)
                {
                    img_Handle.sprite = spr_hand_ON;
                }
                else
                {
                    img_Handle.sprite = spr_hand_OFF;
                }
            });
    }


}
