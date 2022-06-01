using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SignOperatePlane : UIBasePanel
{
    private Image img_Curtain;
    private Image img_Frame;
    private Button btn_Close, btn_SignIn, btn_SignUp, btn_Reset;

    private TMP_InputField input_Email, input_ID, input_Password;
    private Toggle tog_SignIn;
    private TMP_Text txt_Info;

    private CanvasGroup _curtainAlpha;
    private Transform _popUpFrame;

    // 辨别当前是登陆还是注册操作
    private Boolean isSignInorOut;

    protected override void InitPanel()
    {
        base.InitPanel();
        isSignInorOut = false;

        img_Curtain = GetControl<Image>("img_Curtain");
        img_Frame = GetControl<Image>("img_Frame");
        _curtainAlpha = img_Curtain.gameObject.GetComponent<CanvasGroup>();
        _popUpFrame = img_Frame.gameObject.transform;

        btn_Close = GetControl<Button>("btn_Close");
        btn_SignIn = GetControl<Button>("btn_SignIn");
        btn_SignUp = GetControl<Button>("btn_SignUp");
        btn_Reset = GetControl<Button>("btn_Reset");

        input_Email = GetControl<TMP_InputField>("input_Email");
        input_ID = GetControl<TMP_InputField>("input_ID");
        input_Password = GetControl<TMP_InputField>("input_Password");

        txt_Info = GetControl<TMP_Text>("txt_Info");
        txt_Info.text = "已有帐号 ？";
        tog_SignIn = GetControl<Toggle>("tog_SignIn");

        // 设置登陆登出切换
        tog_SignIn.onValueChanged.AddListener((trigger) =>
        {
            input_ID.interactable = !trigger;
            btn_SignIn.interactable = trigger;
            btn_SignUp.interactable = !trigger;
            btn_Reset.interactable = trigger;
        });

        btn_Close.onClick.AddListener(ExitDialog);
        btn_SignIn.onClick.AddListener(OnSignIn);
        btn_SignUp.onClick.AddListener(OnSignUp);
        btn_Reset.onClick.AddListener(OnRestPassword);

        EventCenter.GetInstance().AddEventListener<bool>("Display_SignOperatePlane", DisplayPlane);
        EventCenter.GetInstance().AddEventListener("LoggingInGame", LoggingGame);
        EventCenter.GetInstance().AddEventListener<string>("ReportSignMessage", (msg) => {
            txt_Info.text = msg;
        });

        DisplayPlane(false);
    }

    // 调出重设密码操作
    private void OnRestPassword()
    {
        AudioMgr.GetInstance().PlaySound("UIgmSound", "BtnOnClick");

        if (input_Email.text.Length <= 0)
        {
            txt_Info.text = " 邮箱为空 ！";
            return;
        }

        PlayFabMgr.GetInstance().RestPassword(input_Email.text);
    }

    // 调出登陆操作
    private void OnSignIn()
    {
        AudioMgr.GetInstance().PlaySound("UIgmSound", "BtnOnClick");

        if (input_Email.text.Length <= 0 || input_Password.text.Length < 6)
        {
            txt_Info.text = " 邮箱为空或密码不满6位 ！";
            return;
        }

        isSignInorOut = true;
        PlayFabMgr.GetInstance().Login(input_Email.text, input_Password.text);

    }

    // 调出注册操作
    private void OnSignUp()
    {
        AudioMgr.GetInstance().PlaySound("UIgmSound", "BtnOnClick");

        if (input_Email.text.Length <= 0 || input_ID.text.Length <= 0 || input_Password.text.Length < 6)
        {
            txt_Info.text = " 邮箱，用户名为空或密码不满6位 ！";
            return;
        }

        isSignInorOut = false;
        PlayFabMgr.GetInstance().Register(input_Email.text, input_ID.text, input_Password.text);

    }



    private void OnEnable()
    {
        _curtainAlpha.alpha = 0;
        _curtainAlpha.LeanAlpha(1, 0.6f);

        _popUpFrame.localPosition = new Vector2(-Screen.width, 0);
        _popUpFrame.LeanMoveLocalX(0, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    private void LoggingGame()
    {
        
        DisplayPlane(false);

        if(!isSignInorOut)
            EventCenter.GetInstance().EventTrigger("Display_UserNamePlane", true);
        else
            EventCenter.GetInstance().EventTrigger("Display_LoadingPlane", true);
    }

    private void ExitDialog()
    {
        _curtainAlpha.LeanAlpha(0, 0.6f);
        _popUpFrame.LeanMoveLocalX(-Screen.width, 0.5f)
            .setEaseInExpo().setOnComplete(() => DisplayPlane(false));
    }
}
