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
    // ����״̬���
    public E_Enemy_State state;
    
    // �ƶ����
    [SerializeField]
    private float speed_Walk;
    [SerializeField]
    private float speed_Rotate;

    private Transform self_Enemy;



    // Ѳ�����


    // Ѱ·���

    // ���������

    // ����������

}
