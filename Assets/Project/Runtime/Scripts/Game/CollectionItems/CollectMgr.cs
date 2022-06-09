using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Collectable_Type
{
    Touch, // ����ʽ
    Interact, // ����ʽ
    Hidding // ����״̬
}


/// <summary>
///  �����÷����˼·
///  1. ��¼�����Ĵ���
///  2. ������ʱ������������ӵ���Ч��
///  3. ��¼��ǰ������Ŀ�����
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

    //TODO �����ӷֺ͹鵲�߷ֵĲ���

    private void Start()
    {
        scoreCurrent = 0;
        streakTimer = 0;
        scoreHight = 0; //TODO ���������Ϣ�洢����(srcObj)

    }

    private void FixedUpdate()
    {
        // TODO ��¼������ʼ�ͽ���

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

        // �Է�������������
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

