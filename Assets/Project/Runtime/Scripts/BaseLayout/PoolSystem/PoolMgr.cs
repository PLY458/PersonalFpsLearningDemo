using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;


/// <summary>
/// 缓冲池管理器
/// </summary>
public class PoolMgr : BaseMgr<PoolMgr>
{
    //缓存池容器（查询表）
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    /// <summary>
    /// 检索需要的缓冲池并令池物体出表
    /// </summary>
    public void GetPoolobj(string name, UnityAction<GameObject> callback)
    {

        if (poolDic.ContainsKey(name) && poolDic[name].PoolList.Count > 0)
        {
            callback(poolDic[name].GetObj());
        }
        else if (poolDic.ContainsKey(name) == false)
        {
            ResourcesMgr.GetInstance().LoadAsync<GameObject>(name, (o) =>
            {
                o.name = name;
                callback(o);
            });
        }

    }

    public GameObject GetPoolobj(string name)
    {
        if (poolDic.ContainsKey(name) && poolDic[name].PoolList.Count > 0)
        {
            return poolDic[name].GetObj();
        }

        return null;
    }


    /// <summary>
    /// 检索需要的缓冲池并令池物体入表
    /// </summary>
    public void PushPoolobj(string name, GameObject obj)
    {
        
        if (poolDic.ContainsKey(name))
        {
            //Debug.Log("得到需要回收的列表：" + name);
            poolDic[name].PushObj(obj);

        }
        else
        {
            //Debug.Log("回收物体姓名为：" + name);
            poolDic.Add(name, new PoolData(obj));
        }

    }

    //清除所有缓冲池记录
    public void Clear()
    {
        poolDic.Clear();
    }
}