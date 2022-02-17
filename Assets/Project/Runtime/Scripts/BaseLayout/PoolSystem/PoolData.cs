using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;


/// <summary>
/// 缓冲池数据集
/// </summary>
public class PoolData
{
    //挂载的池头
    public GameObject fatherObj;
    //对象的池表
    public Stack<GameObject> PoolList;

    /// <summary>
    ///   初始化池容器单元
    /// </summary>
    /// <param name="obj">首个游戏物体</param>
    /// <param name="poolObj">需要依附的池物体</param>
    public PoolData(GameObject obj, GameObject poolObj)
    {
        //给容器头和容器表进行配置
        fatherObj = new GameObject(obj.name + "pool");
        //依附在主池上
        fatherObj.transform.parent = poolObj.transform;
        //初始化子池
        PoolList = new Stack<GameObject>();
        PushObj(obj);
    }

    /// <summary>
    /// 物体存池
    /// </summary>
    public void PushObj(GameObject obj)
    {
        //失活
        obj.SetActive(false);
        //存入
        PoolList.Push(obj);
        //重定位在池头物体上
        obj.transform.SetParent(fatherObj.transform);
    }

    /// <summary>
    /// 物体出池
    /// </summary>
    public GameObject GetObj()
    {
        GameObject obj = null;
        //取出头一个
        obj = PoolList.Pop();
        //激活，让其显示
        obj.SetActive(true);
        //子对象脱离池对象
        obj.transform.SetParent(null);
        return obj;
    }

    /// <summary>
    /// 清除池所有物体
    /// </summary>
    public void Clear()
    {
        PoolList.Clear();
    }
}

