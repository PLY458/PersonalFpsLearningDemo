using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 继承了 MonoBehaviour 的单例模式
// 保证场景中只有一个物体搭载
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T GetInstance() { return instance; }

    protected virtual void Awake() { instance = this as T; }

}
