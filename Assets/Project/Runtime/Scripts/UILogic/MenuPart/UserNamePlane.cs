using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserNamePlane : UIBasePanel
{
    private Image img_Curtain;
    private Image img_Frame;

    private CanvasGroup _curtainAlpha;
    private Transform _inputFrame;

    private TMP_InputField inputField_PlayerName;

    private Button btn_Close;
    private Button btn_Ok;

    private Toggle tog_LanUS;
    private Toggle tog_LanCHS;
    private Toggle tog_LanJP;

    private TMP_Text txt_tips;

    //需要传输得地区信息
    private string language = default;

    protected override void InitPanel()
    {
        base.InitPanel();

        inputField_PlayerName = GetControl<TMP_InputField>("inputField_PlayerName");

        //btn_Close = GetControl<Button>("btn_Close");
        btn_Ok = GetControl<Button>("btn_Ok");

        tog_LanUS = GetControl<Toggle>("tog_LanUS");
        tog_LanJP = GetControl<Toggle>("tog_LanJP");
        tog_LanCHS = GetControl<Toggle>("tog_LanCHS");

        SetBtnTriggers();

        language = "";

        img_Curtain = GetControl<Image>("img_Curtain");
        img_Frame = GetControl<Image>("img_Frame");

        txt_tips = GetControl<TMP_Text>("txt_tips");

        _curtainAlpha = img_Curtain.gameObject.GetComponent<CanvasGroup>();
        _inputFrame = img_Frame.gameObject.transform;

        EventCenter.GetInstance().AddEventListener<bool>("Display_UserNamePlane", DisplayPlane);

        DisplayPlane(false);
    }

    private void SetBtnTriggers()
    {
        tog_LanUS.onValueChanged.AddListener((msg)=> {
            AudioMgr.GetInstance().PlaySound("UIgmSound", "BtnHighLight");

            if (msg)
                language = "en";
        });

        tog_LanCHS.onValueChanged.AddListener((msg) => {
            AudioMgr.GetInstance().PlaySound("UIgmSound", "BtnHighLight");

            if (msg)
                language = "zh";
        });

        tog_LanJP.onValueChanged.AddListener((msg) => {
            AudioMgr.GetInstance().PlaySound("UIgmSound", "BtnHighLight");

            if (msg)
                language = "ja";
        });

        btn_Ok.onClick.AddListener(()=> {
            AudioMgr.GetInstance().PlaySound("UIgmSound", "BtnOnClick");

            if (inputField_PlayerName.text.Length <= 0 || language.Length <= 0)
            {
                txt_tips.text = "当前玩家名或地区选择为空！";
                 return;
            }
            Debug.Log("开始登陆");
            ExitDialog();
            PlayFabMgr.GetInstance().SubmitPlayerProfile(inputField_PlayerName.text, language);
            EventCenter.GetInstance().EventTrigger("Display_LoadingPlane", true);
        });
    }


    private void OnEnable()
    {
        _curtainAlpha.alpha = 0;
        _curtainAlpha.LeanAlpha(1, 0.6f);

        _inputFrame.localPosition = new Vector2(-Screen.width, 0);
        _inputFrame.LeanMoveLocalX(0, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    private void ExitDialog()
    {
        _curtainAlpha.LeanAlpha(0, 0.6f);
        _inputFrame.LeanMoveLocalX(-Screen.width, 0.5f)
            .setEaseInExpo().setOnComplete(() => DisplayPlane(false));
    }
}
