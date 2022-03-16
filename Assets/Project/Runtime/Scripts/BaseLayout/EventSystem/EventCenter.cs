using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;

/// <summary>
/// 封装子类的接口
/// </summary>
public interface IEventInfo
{

}

/// <summary>
/// 被封装的子类，为游戏事件委托方法群
/// 利用泛型避免拆装箱操作
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

/*
 *事件处理中心 单例对象
 *1.Dicionary
 *2.委托
 * 3.观察者设计模式
 * 4.泛型
 * 5.里氏转换原则
 */
public class EventCenter:BaseMgr<EventCenter>
{
    //key -- 事件名
    //value -- 监听相应事件的泛型委托函数(泛型方便传递游戏物体参数)
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();
    
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="action">处理事件的委托</param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {

        if (eventDic.ContainsKey(name))
        {
            //有加入多播队列
            (eventDic[name] as EventInfo<T>).actions += action;
            Debug.Log("原事件："+ name + " 加入新活动：" + action.ToString());
        }
        else
        {
            //没有创建新的队列
            eventDic.Add(name,new EventInfo<T>(action));
            Debug.Log("加入新事件：" + name + " 首个活动：" + action.ToString());
            
        }
    }

    /// <summary>
    /// 不需要传递参数的事件添加方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddEventListener(string name, UnityAction action)
    {

        if (eventDic.ContainsKey(name))
        {
            //有加入多播队列
            (eventDic[name] as EventInfo).actions += action;
            Debug.Log("原事件：" + name + " 加入新活动：" + action.Method.Name.ToString());
        }
        else
        {
            //没有创建新的队列
            eventDic.Add(name,new EventInfo(action));
            Debug.Log("加入新事件：" + name + " 首个活动：" + action.Method.Name.ToString());
        }
    }

    /// <summary>
    /// 事件清除
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="action">处理事件的委托</param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
        Debug.LogWarning("未讯查到对应事件? ：" + name);
    }

    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;
        }
        Debug.LogWarning("未讯查到对应事件? ：" + name);
    }


    /// <summary>
    ///  事件触发
    /// </summary>
    /// <param name="name">对应名称的事件触发</param>
    /// <param name="info">事件传递的物体参数</param>
    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo<T>).actions != null)
            {
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
            }
            else
            {
                Debug.Log("对应活动不存在！");
            }
        }
        else
        {
            Debug.LogWarning("未讯查到对应事件? ：" + name);
        }
    }

    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo).actions != null)
            {
                (eventDic[name] as EventInfo).actions.Invoke();
            }
            else
            {
                Debug.Log("对应活动不存在！");
            }
        }
        else
        {
            Debug.LogWarning("未讯查到对应事件? ：" + name);
        }
    }

    /// <summary>
    /// 清空所又事件
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}
