using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

        //EventCenter.GetInstance().EventTrigger("Display_ExitPlane", false);
        //EventCenter.GetInstance().EventTrigger("Display_SettingPlane", false);
        //EventCenter.GetInstance().EventTrigger("Display_LoadingPlane", false);
    }

    private void TitleZoomAnimate()
    {
        txt_DemoTitle.transform.LeanScale(new Vector2(1.5f, 1.5f), 0.9f).setEaseInCirc().setLoopPingPong();
    }

    private void SetButtonDynamicEffect(Button btn)
    {
        Vector2 scaleVec2 = new Vector2(1.3f, 1.3f);
        
    }

    private void Click_BtnExit()
    {
        AudioMgr.GetInstance().PlaySound("UIgmSound","BtnOnClick");
        EventCenter.GetInstance().EventTrigger("Display_ExitPlane", true);
        
    }

    private void Click_BtnStart()
    {
        AudioMgr.GetInstance().PlaySound("UIgmSound", "BtnOnClick");
        EventCenter.GetInstance().EventTrigger("Display_SignOperatePlane", true);
        
    }

    private void Click_BtnSetting()
    {
        AudioMgr.GetInstance().PlaySound("UIgmSound", "BtnOnClick");
        EventCenter.GetInstance().EventTrigger("Display_SettingPlane", true);
    }

}
