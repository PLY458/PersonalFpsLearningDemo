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


    public void GatherPoint(int score, E_Collectable_Type type)
    {
        // ��������ʱ����ζ
        streakTimer = streakExpiredTime;
        // �Է�������������
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

