using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePlane : UIBasePanel
{
    private TMP_Text txt_scoreCurrent;
    private TMP_Text txt_scoreHight;
    private TMP_Text txt_streakCombo;
    private Slider slid_streakBar;
    private GameObject ctObject, hiObject, coObject;

    private int tempScore;
    private Queue<int> scoreQueue = new Queue<int>();
    private Queue<int> comboQueue = new Queue<int>();
    // UI特效
    [SerializeField] private float refreshTime = 0.8f;
    [SerializeField] private float jumpEffectTime = 0.4f;

    Vector3 testScale = new Vector3(1.8f, 1.8f, 0);

    // 分数跳动时间间隔
    [SerializeField] private float jumpBtwTime = 0.5f; // >=jumpEffectTime
    // 分数跳动次数缓存
    private int jumpBtwBuffer = default;
    private float btwTimer;
    
    protected override void InitPanel()
    {
        base.InitPanel();
        txt_scoreCurrent = GetControl<TMP_Text>("txt_scoreCurrent");
        txt_scoreHight = GetControl<TMP_Text>("txt_scoreHight");
        txt_streakCombo = GetControl<TMP_Text>("txt_streakCombo");

        slid_streakBar = GetControl<Slider>("slid_streakBar");

        tempScore = 0;
        txt_scoreCurrent.text = 0 + " Points";
        txt_scoreHight.text = "HightPoints: " + 0 ;

        ctObject = txt_scoreCurrent.gameObject;
        hiObject = txt_scoreHight.gameObject;
        coObject = txt_streakCombo.gameObject;
        coObject.SetActive(false);

        btwTimer = 0.0f;
        jumpBtwBuffer = 0;

        slid_streakBar.value = 0.0f;
        EventCenter.GetInstance().AddEventListener<int>("SetUICurrentScore", SetCurrentScore);
        EventCenter.GetInstance().AddEventListener<int>("SetUIHightScore", SetHightScore);
        EventCenter.GetInstance().AddEventListener<float>("SetUIStreakBar", RefreshStreakBar);
        EventCenter.GetInstance().AddEventListener<int>("SetUICurrentCombo", SetCurrentCombo);
    }

    // TODO 分数跳动效果会出现偏移

    private void OnValidate()
    {
        if (jumpBtwTime < jumpEffectTime)
            jumpBtwTime = jumpEffectTime;
    }

    // 将跳动次数迁移到refreshPlane中
    public override void RefreshPlane()
    {
        base.RefreshPlane();
        // 为特效间隔计时
        if (jumpBtwBuffer > 0)
        {
            if (btwTimer <= 0)
            {

                LeanTween.cancel(ctObject);
                LeanTween.scale(ctObject, testScale, jumpEffectTime).setEasePunch();

                int targetScore = scoreQueue.Dequeue();
                LeanTween.value(tempScore, targetScore, refreshTime)
                    .setEase(LeanTweenType.easeInOutQuad)
                    .setOnUpdate((float _score) =>
                    {
                        txt_scoreCurrent.text = targetScore + " Points";
                        if (comboQueue.Count > 0)
                            txt_streakCombo.text = "x" + comboQueue.Dequeue();                        
                        tempScore = targetScore;
                    });

                // ???
                //if (coObject.activeSelf)
                //{
                //    LeanTween.cancel(coObject);
                //    LeanTween.scale(coObject, testScale, jumpEffectTime).setEasePunch();

                //    int targetCombo = comboQueue.Dequeue();
                //    LeanTween.value(tempCombo, targetCombo, refreshTime)
                //        .setEase(LeanTweenType.easeInOutQuad)
                //        .setOnUpdate((float _score) =>
                //        {
                //            txt_streakCombo.text = "x" + targetCombo;
                //            tempCombo = targetCombo;
                //        });
                //}

                jumpBtwBuffer--;
                btwTimer = jumpBtwTime;

            }
            else
            {
                btwTimer -= Time.deltaTime;
            }

        }
        else
        {
            btwTimer = 0.0f;
        }


        
    }

    public void SetCurrentScore(int targetscore)
    {
        // 测试playfab登陆
        PlayFabMgr.GetInstance().Login();

        jumpBtwBuffer++;
        scoreQueue.Enqueue(targetscore);
    }

    public void SetHightScore(int score)
    {
        GameObject g_txt = txt_scoreHight.gameObject;

        LeanTween.cancel(g_txt);

        LeanTween.scale(g_txt, testScale, 0.6f).setEasePunch();

        txt_scoreHight.text = "HightPoints: " + score;
    }

    public void RefreshStreakBar(float percent)
    {
        slid_streakBar.value = percent;
    }

    public void SetCurrentCombo(int comboCount)
    {
        if (comboCount >= 2)
        {
            coObject.SetActive(true);
            Debug.Log("有效连击数据：" + comboCount);
            comboQueue.Enqueue(comboCount);
        }
        else
        {
            comboQueue.Clear();
            coObject.SetActive(false);
        }
    }
}
