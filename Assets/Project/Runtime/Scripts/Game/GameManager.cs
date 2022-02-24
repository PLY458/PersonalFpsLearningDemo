using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonAutoMono<GameManager>
{
    public void InitGame()
    {
        Debug.Log("初始化游戏进程");
    }
}
