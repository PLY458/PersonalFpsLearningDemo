using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// 1.场景异步切换
/// 2.协程
/// 3.委托
/// </summary>
public class ScenceMgr : BaseMgr<ScenceMgr>
{
    /// <summary>
    /// 同步切换场景
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param fun="UnityAction">补充执行的方法</param>
    public void LoadScenesyn(string name, UnityAction fun = null)
    {
        //加载场景
        SceneManager.LoadScene(name);
        //需要补充执行的方法
        fun();
    }

    /// <summary>
    /// 异步切换场景
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="fun">补充执行的方法</param>
    public void LoadSceneAsyn(string name, UnityAction fun = null)
    {
        GameManager.GetInstance().StartCoroutine(ReallyLoadSceneAsyn(name, fun));
    }

    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction fun = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);

        //得到场景加载的一个进度
        while (!ao.isDone)
        {
            //事件中心 向外分发 进度情况 外面想用就用
            EventCenter.GetInstance().EventTrigger("Repear_LoadingScene", ao.progress);
            //这里更新进度条
            yield return ao.progress;
        }

        yield return ao;

        //补充方法
        fun();
    }
}
