using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Collection_Type
{
    Coin,
    Cameo,
}

public class CollectMgr : SingletonMono<CollectMgr>
{

    private int score_Current;

    private int score_Hight;

    //TODO �����ӷֺ͹鵲�߷ֵĲ���

    private void Start()
    {
        score_Current = 0;

        score_Hight = 0; //TODO ���������Ϣ�洢����(srcObj)
    }

    public void AddPoint(E_Collection_Type type)
    {
        score_Current += 100;
        EventCenter.GetInstance().EventTrigger("SetUICurrentScore", score_Current);
        StartCoroutine(OperatePointSwitch());
    }

    private IEnumerator OperatePointSwitch()
    {
        yield return new WaitForSeconds(1.5f);
        if (score_Current > score_Hight)
        {
            score_Hight = score_Current;
            EventCenter.GetInstance().EventTrigger("SetUIHightScore", score_Hight);
        }
        score_Current = 0;
        EventCenter.GetInstance().EventTrigger("SetUICurrentScore", score_Current);
    }
}


public static class CollectPoints
{
        

}