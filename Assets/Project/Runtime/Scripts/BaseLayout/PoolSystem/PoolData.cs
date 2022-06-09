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

    //对象的池表
    public Stack<GameObject> PoolList;

    /// <summary>
    ///   初始化池容器单元
    /// </summary>
    /// <param name="obj">首个游戏物体</param>
    public PoolData(GameObject obj)
    {
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

