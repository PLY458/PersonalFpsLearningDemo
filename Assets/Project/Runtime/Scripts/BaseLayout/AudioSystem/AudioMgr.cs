using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 音效和音乐管理器
/// </summary>
public class AudioMgr : BaseMgr<AudioMgr>
{
    //背景音乐
    private AudioSource bkMusic = null;
    //BGM音量
    private float bkValue = 1;

    //声源GameObj
    private GameObject soundObj = null;
    //音效队列
    private List<AudioSource> soundList = new List<AudioSource>();
    //音效音量
    private float soundValue = 1;

    public AudioMgr()
    {
        // GameMgr.GetInstance().AddUpdateListener(Update);
    }

    /// <summary>
    /// 重写Update，在运行当中时刻判断是否需要删除队列中不再用的音效
    /// </summary>
    private void Update()
    {
        for (int i = soundList.Count - 1; i >= 0; --i)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    #region 背景音乐控制
    /// <summary>
    /// 播放背景曲
    /// </summary>
    /// <param name="name"></param>
    public void PlayBkMusic(string name)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BGMusic";
            bkMusic = obj.AddComponent<AudioSource>();

        }
        //当资源加载完后，回调一个音效组件
        ResourcesMgr.GetInstance().LoadAsync<AudioClip>("AudioRes/BGM/"+name, (clip) =>
        {
            bkMusic.clip = clip;
            bkMusic.volume = bkValue;
            bkMusic.Play();
        });
    }

    /// <summary>
    /// 暂停背景曲
    /// </summary>
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }


    /// <summary>
    /// 停止背景曲
    /// </summary>
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    /// <summary>
    /// 改变背景曲音量
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBKValue(float v)
    {
        bkValue = v;
        if(bkMusic == null)
            return;
        bkMusic.volume = bkValue;
    }
    #endregion

    #region 音效控制
    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string name, bool isloop,UnityAction<AudioSource> callBack = null)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Sound";

        }
        ResourcesMgr.GetInstance().LoadAsync<AudioClip>("AudioRes/Sound/" + name, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = isloop;
            source.volume = soundValue;
            source.Play();
            soundList.Add(source);
            callBack?.Invoke(source);
        });
    }

    /// <summary>
    /// 音效音量大小调节
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundValue(float value)
    {
        soundValue = value;
    }


    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
        }
    }
    #endregion
}
