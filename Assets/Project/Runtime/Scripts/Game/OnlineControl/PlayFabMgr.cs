using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;


public class PlayFabMgr : SingletonAutoMono<PlayFabMgr>
{
    public void Login()
    {
        var request = new LoginWithCustomIDRequest {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        Debug.Log(" 登陆/用户创建测试成功 ");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log(" 登陆/用户创建测试失败 ");
        Debug.Log(error.GenerateErrorReport());
    }
}
