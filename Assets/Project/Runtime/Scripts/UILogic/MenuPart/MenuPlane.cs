using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuPlane : UIBasePanel
{
    private TMP_Text txt_DemoTitle;

    private Button btn_ExitGame;
    private Button btn_StartGame;
    private Button btn_GameSetting;

    protected override void InitPanel()
    {
        base.InitPanel();
        txt_DemoTitle = GetControl<TMP_Text>("txt_DemoTitle");

        btn_ExitGame = GetControl<Button>("btn_ExitGame");
        btn_ExitGame.onClick.AddListener(Click_BtnExit);
        btn_StartGame = GetControl<Button>("btn_StartGame");
        btn_StartGame.onClick.AddListener(Click_BtnStart);
        btn_GameSetting = GetControl<Button>("btn_GameSetting");
        btn_GameSetting.onClick.AddListener(Click_BtnSetting);

        TitleZoomAnimate();

        EventCenter.GetInstance().EventTrigger("Display_ExitPlane", false);
        EventCenter.GetInstance().EventTrigger("Display_SettingPlane", false);
    }

    private void TitleZoomAnimate()
    {
        txt_DemoTitle.transform.LeanScale(new Vector2(1.3f, 1.3f), 0.9f).setEaseInCirc().setLoopPingPong();
    }

    private void Click_BtnExit()
    {
        Debug.Log("∫ÙªΩÕÀ≥ˆ“≥√Ê ! !");
        EventCenter.GetInstance().EventTrigger("Display_ExitPlane", true);
    }

    private void Click_BtnStart()
    {
        
    }

    private void Click_BtnSetting()
    {
        Debug.Log("∫ÙªΩ…Ë÷√“≥√Ê ! !");
        EventCenter.GetInstance().EventTrigger("Display_SettingPlane", true);
    }

}
