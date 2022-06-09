using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Collectable_Type
{
    Touch, // 触碰式
    Interact, // 交互式
    Hidding // 隐藏状态
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

    private void FixedUpdate()
    {
        // TODO 记录连击开始和结束

        if (streakTimer > 0)
        {
            streakTimer -= Time.fixedDeltaTime;
            if (streakTimer <= 0)
            {
                StartCoroutine(OperatePointSwitch());
                return;
            }
            float percent = streakTimer / streakExpiredTime;
            if (Mathf.Abs(percent) > Mathf.Epsilon)
            {
                EventCenter.GetInstance().EventTrigger("SetUIStreakBar", percent * 100.0f);

            }
        }

    }

    public void GatherPoint(int score, E_Collectable_Type type)
    {
        if (type <= E_Collectable_Type.Touch)
        {
            streakTimer = streakExpiredTime;
        }

        // 对分数做连击处理
        streakCount++;
        if (streakCount > 1)
            EventCenter.GetInstance().EventTrigger("SetUICurrentCombo", streakCount);
        //scoreCurrent += score * streakCount;

        scoreCurrent += score;

        AudioMgr.GetInstance().PlaySound("Collect", "CoinCollect");
        EventCenter.GetInstance().EventTrigger("SetUICurrentScore", scoreCurrent);
        
    }


    private IEnumerator OperatePointSwitch()
    {
        yield return null;
        if ((scoreCurrent * streakCount) > scoreHight)
        {
            scoreHight = scoreCurrent * streakCount;
            EventCenter.GetInstance().EventTrigger("SetUIHightScore", scoreHight);
        }
        scoreCurrent = 0;
        streakCount = 0;

        AudioMgr.GetInstance().PlaySound("Collect", "ScoreSum");
        EventCenter.GetInstance().EventTrigger("SetUICurrentScore", scoreCurrent);
        EventCenter.GetInstance().EventTrigger("SetUICurrentCombo", streakCount);
    }
}

