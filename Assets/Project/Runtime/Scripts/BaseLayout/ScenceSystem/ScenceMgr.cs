using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// �����л�ģ��
/// 1.�����첽�л�
/// 2.Э��
/// 3.ί��
/// </summary>
public class ScenceMgr : BaseMgr<ScenceMgr>
{
    /// <summary>
    /// ͬ���л�����
    /// </summary>
    /// <param name="name">������</param>
    /// <param fun="UnityAction">����ִ�еķ���</param>
    public void LoadScenesyn(string name, UnityAction fun = null)
    {
        //���س���
        SceneManager.LoadScene(name);
        //��Ҫ����ִ�еķ���
        fun();
    }

    /// <summary>
    /// �첽�л�����
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="fun">����ִ�еķ���</param>
    public void LoadSceneAsyn(string name, UnityAction fun = null)
    {
        GameManager.GetInstance().StartCoroutine(ReallyLoadSceneAsyn(name, fun));
    }

    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction fun = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);

        //�õ��������ص�һ������
        while (!ao.isDone)
        {
            //�¼����� ����ַ� ������� �������þ���
            EventCenter.GetInstance().EventTrigger("Repear_LoadingScene", ao.progress);
            //������½�����
            yield return ao.progress;
        }

        yield return ao;

        //���䷽��
        fun();
    }
}
