using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScoreRankBoard", fileName = "RankList") ]
public class RankListData : ScriptableObject
{
    public List<PlayerRankData> player_List;

 
}
