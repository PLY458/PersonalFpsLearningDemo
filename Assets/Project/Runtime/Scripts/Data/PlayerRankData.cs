using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Player_Nation
{
    CN,
    EN,
    RU
}


[CreateAssetMenu(menuName = "ScoreRankBoard", fileName = "RankData")]
public class PlayerRankData : ScriptableObject
{
    public string playerName;
    public E_Player_Nation playerNation;
    public Sprite playerCharacter, playerFrame;


}

//[System.Serializable]
//public class PlayerInfo
//{
    
//}
