using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectObject : MonoBehaviour
{
    // �ռ���������
    [SerializeField]
    private E_Collectable_Type typeCollactable;

    // �ռ���õķ���
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
