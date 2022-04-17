using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Collectable_Type
{
    Touch,
    Interact,
    Destory
}


/// <summary>
///  连击得分设计思路
///  1. 记录连击的次数
///  2. 连击计时器保障连击添加的有效性
///  3. 记录当前分数和目标分数
/// </summary>
public class CollectMgr : SingletonMono<CollectMgr>
{
    [SerializeField]
    private float streakExpiredTime = 2.0f;
    [SerializeField]
    private int streakCount = 0;

    private float streakTimer;

    private int scoreCurrent;
    private int scoreHight;

    //TODO 制作加分和归挡高分的操作

    private void Start()
    {
        scoreCurrent = 0;
        streakTimer = 0;
        scoreHight = 0; //TODO 借助玩家信息存储分数(srcObj)
    }


    public void GatherPoint(int score, E_Collectable_Type type)
    {
        // 将连击计时器归味
        streakTimer = streakExpiredTime;
        // 对分数做连击处理
        streakCount++;
        scoreCurrent += score * streakCount;

        //EventCenter.GetInstance().EventTrigger("SetUICurrentScore", scoreCurrent);
        //StartCoroutine(OperatePointSwitch());
    }

    private IEnumerator OperatePointSwitch()
    {
        yield return new WaitForSeconds(1.5f);
        if (scoreCurrent > scoreHight)
        {
            scoreHight = scoreCurrent;
            EventCenter.GetInstance().EventTrigger("SetUIHightScore", scoreHight);
        }
        scoreCurrent = 0;
        EventCenter.GetInstance().EventTrigger("SetUICurrentScore", scoreCurrent);
    }
}

