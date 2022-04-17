using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Enemy_State
{
        Patroling,
        Chase,
        Attack,
        Destory
}


public class Enemy : MonoBehaviour
{
    // 敌人状态相关
    public E_Enemy_State state;
    
    // 移动相关
    [SerializeField]
    private float speed_Walk;
    [SerializeField]
    private float speed_Rotate;

    private Transform self_Enemy;



    // 巡逻相关


    // 寻路相关

    // 检测玩家相关

    // 攻击玩家相关

}
