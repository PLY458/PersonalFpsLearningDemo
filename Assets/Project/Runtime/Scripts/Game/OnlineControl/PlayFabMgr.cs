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
        Debug.Log(" ��½/�û��������Գɹ� ");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log(" ��½/�û���������ʧ�� ");
        Debug.Log(error.GenerateErrorReport());
    }
}
