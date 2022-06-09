using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using Object = UnityEngine.Object;


/// <summary>
/// 资源加载模块
/// 1.异步加载
/// 2.委托和Lambda表达式
/// 3.协程
/// 4.泛型
/// </summary>
public class ResourcesMgr : BaseMgr<ResourcesMgr>
{
    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="name">资源名称</param>
    /// <returns></returns>
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        //判断资源类型，返回相应的对象
        if (res is GameObject)//游戏物体就返回实例化对象
            return Object.Instantiate(res);
        else//TextAsset AudioClip文件
        {
            return res;
        }

    }


    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="name">资源名称</param>
    /// <param name="callback">回调方法</param>
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        //开启异步加载的协程
        // GameMgr.GetInstance().StartCoroutine(ReallyLoadAsync(name,callback));
        GameManager.GetInstance().StartCoroutine(ReallyLoadAsync(name, callback));
    }

    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if (r.asset is GameObject)
            callback(Object.Instantiate(r.asset) as T);
        else
            callback(r.asset as T);
    }

}
