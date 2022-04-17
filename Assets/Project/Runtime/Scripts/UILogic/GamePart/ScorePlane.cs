using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePlane : UIBasePanel
{
    private TMP_Text txt_scoreCurrent;
    private TMP_Text txt_scoreHight;
    private Button btn_scoreTest;
    private GameObject ctObject, hiObject;


    private int tempScore;

    [SerializeField] private float refreshTime = 0.8f;
    [SerializeField] private float jumpEffectTime = 0.4f;

    Vector3 testScale = new Vector3(1.8f, 1.8f, 0);

    protected override void InitPanel()
    {
        base.InitPanel();
        txt_scoreCurrent = GetControl<TMP_Text>("txt_scoreCurrent");
        txt_scoreHight = GetControl<TMP_Text>("txt_scoreHight");
        btn_scoreTest = GetControl<Button>("btn_scoreTest");

        txt_scoreCurrent.text = 0 + " Points";
        txt_scoreHight.text = "HightPoints: " + 0 ;
        tempScore = 0;

        ctObject = txt_scoreCurrent.gameObject;
        hiObject = txt_scoreHight.gameObject;


        btn_scoreTest.onClick.AddListener(() =>
        {
            // 使用协程/异步及时中断未完成的重复点击操作

            LeanTween.cancel(ctObject);

            LeanTween.scale(ctObject, testScale, jumpEffectTime).setEasePunch();
        });
        //EventCenter.GetInstance().AddEventListener<int>("SetUICurrentScore", RefreshCurrentScore);
        //EventCenter.GetInstance().AddEventListener<int>("SetUIHightScore", RefreshHightScore);
    }

    // TODO 分数跳动效果会出现偏移

    public void RefreshCurrentScore(int targetscore)
    {
        GameObject g_txt = txt_scoreCurrent.gameObject;

        LeanTween.cancel(g_txt);

        LeanTween.scale(g_txt, testScale, jumpEffectTime).setEasePunch();

        if (targetscore > tempScore)
        {
            LeanTween.value(tempScore, targetscore, refreshTime)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnUpdate((float _score) =>
                {
                    txt_scoreCurrent.text = targetscore + " Points";
                });
        }
        else
        {
            txt_scoreCurrent.text = 0 + " Points";
        }

        tempScore = targetscore;

    }

    public void RefreshHightScore(int score)
    {
        GameObject g_txt = txt_scoreHight.gameObject;

        LeanTween.cancel(g_txt);

        LeanTween.scale(g_txt, testScale, 0.6f).setEasePunch();

        txt_scoreHight.text = "HightPoints: " + score;
    }
}
