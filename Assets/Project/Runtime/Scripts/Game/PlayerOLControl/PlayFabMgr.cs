using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ProfilesModels;
using System;


public class PlayFabMgr : SingletonAutoMono<PlayFabMgr>
{

    private List<PlayerInfo> playRankList;

    public List<PlayerInfo> PlayRankList { get => playRankList; }


    #region �û���½����
    // �û�ע�ᣬ��½
    public void Register(string email, string name, string pass)
    {

        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Username = name,
            Password = pass,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnSignError);

    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log(" Registered and logged in! " + result.Username);
        // �����û���Ϣ����
        EventCenter.GetInstance().EventTrigger("LoggingInGame");
    }

    public void Login(string email, string pass)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = pass
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnSignError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log(" ��½���Գɹ�: " + result.PlayFabId);
        EventCenter.GetInstance().EventTrigger("LoggingInGame");
    }

    public void RestPassword(string email)
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = email,
            TitleId = "7B1E0"
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnSignError);
    }

    private void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("Password reset mail sent !");
        EventCenter.GetInstance().EventTrigger("ReportSignMessage", "�ѶԸ����䷢����֤��");
    }


    void OnSignError(PlayFabError error)
    {
        Debug.Log(" ��½/�û���������ʧ�� ");
        // TODO ��������Ϣ����ui��� error.message

        Debug.Log(error.GenerateErrorReport());
    }

    #endregion


    #region ����û������޸�

    // ����û������޸�

    public void SubmitPlayerProfile(string name, string lang)
    {
        Debug.Log("���µ�������ƣ�" + name + "�����ԣ�" + lang);

        var requestName = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };

        
        var requestLang = new SetProfileLanguageRequest
        {
            Language = lang
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(requestName, OnDisplayNameUpadate, OnUpdatePlayerError);
        PlayFabProfilesAPI.SetProfileLanguage(requestLang, OnLanguageSet, OnUpdatePlayerError);

    }

    private void OnDisplayNameUpadate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log(" Updated display name! ");
        
    }

    private void OnLanguageSet(SetProfileLanguageResponse response)
    {
        Debug.Log(" Updated language! ");

    }

    void OnUpdatePlayerError(PlayFabError error)
    {
        Debug.Log(" �����Ϣ����ʧ�� ");

        Debug.Log(error.GenerateErrorReport());
    }

        #endregion


    #region ������а�

        // ���а�
        public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "PlayerScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnleaderboardUpadte, OnSignError);
    }

    private void OnleaderboardUpadte(UpdatePlayerStatisticsResult result)
    {
        Debug.Log(" �ϴ����а����ݳɹ��� ");
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "PlayerScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnleaderboardGet, OnSignError);

    }

    private void OnleaderboardGet(GetLeaderboardResult result)
    {
        if (playRankList == null)
        {
            playRankList = new List<PlayerInfo>();
        }

        if (playRankList.Count <= result.Leaderboard.Count)
        {
            playRankList.Clear();
            foreach (var item in result.Leaderboard)
            {
                Debug.Log("Place: | " + item.Position + " | " + item.DisplayName + " | " + item.StatValue);
                playRankList.Add(new PlayerInfo(item.Position, item.DisplayName, item.StatValue));
            }
        }

        EventCenter.GetInstance().EventTrigger("GenerateRankCell");
    }

    #endregion

}


// ���а��������Ϣ
public class PlayerInfo
{
    public int indexPos;
    public string playName;
    public int playScore;

    public PlayerInfo(int pos, string id, int score)
    {
        indexPos = pos;
        playName = id;
        playScore = score;
    }
}