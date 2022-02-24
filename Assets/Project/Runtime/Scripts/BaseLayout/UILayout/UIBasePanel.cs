using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// UI面板基类
/// 找到该面板下的全部子空间对象
/// 提供显示，隐藏等功能
/// </summary>
public class UIBasePanel : MonoBehaviour
{
   //里氏转换换原则 存储所有的子控件  
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();


    protected virtual void Awake()
    {

        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<TMP_Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<Scrollbar>();
        InitPanel();
    }

    protected virtual void Update()
    {
        RefreshPlane();
    }

    /// <summary>
    /// 必要的面板初始化
    /// </summary>
    protected virtual void InitPanel()
    {
        
    }


    /// <summary>
    /// 显示面板
    /// </summary>
    public virtual void RefreshPlane()
    {

    }


    /// <summary>
    /// 隐藏面板
    /// </summary>
    protected virtual void DisplayPlane(bool d_Order)
    {
        gameObject.SetActive(d_Order);
    }

    /// <summary>
    ///   获得对应名称子控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(controlName))
        {
            for (int i = 0; i < controlDic[controlName].Count; i++)
            {
                if (controlDic[controlName][i] is T)
                {
                    //获得所有的组件序列
                    return controlDic[controlName][i] as T;
                }
            }
        }
        Debug.LogError("未查到对应UI组件：" + controlName);
        return null;
    }


    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    protected void FindChildrenControl<T>() where  T:UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        string objName;
        for (int i = 0; i < controls.Length; i++)
        {
            objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
            {
                controlDic[objName].Add(controls[i]);
            }
            else
            {
                controlDic.Add(objName,new List<UIBehaviour>(){controls[i]});
            }
            
        }
    }
}
