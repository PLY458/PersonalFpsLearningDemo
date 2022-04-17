using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectObject : MonoBehaviour
{
    // 收集方法类型
    [SerializeField]
    private E_Collectable_Type typeCollactable;

    // 收集获得的分数
    [SerializeField]
    private int scoreObj;


    protected virtual void CallToGatherPoints()
    {
        CollectMgr.GetInstance().GatherPoint(scoreObj, typeCollactable);
    }

    protected virtual void CallToGatherCollections()
    {

    }

}
