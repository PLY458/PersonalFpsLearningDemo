using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePlane : UIBasePanel
{
    private TMP_Text txt_scoreCurrent;
    private TMP_Text txt_scoreHight;

    Vector3 testScale = new Vector3(1.8f, 1.8f, 0);

    protected override void InitPanel()
    {
        base.InitPanel();
        txt_scoreCurrent = GetControl<TMP_Text>("txt_scoreCurrent");
        txt_scoreHight = GetControl<TMP_Text>("txt_scoreHight");

        txt_scoreCurrent.text = 0 + " Points";
        txt_scoreHight.text = "HightPoints: " + 0 ;

        EventCenter.GetInstance().AddEventListener<int>("SetUICurrentScore", RefreshCurrentScore);
        EventCenter.GetInstance().AddEventListener<int>("SetUIHightScore", RefreshHightScore);
    }

    // TODO 分数跳动效果会出现偏移

    public void RefreshCurrentScore(int score)
    {
        GameObject g_txt = txt_scoreCurrent.gameObject;

        LeanTween.cancel(g_txt);

        LeanTween.scale(g_txt, testScale, 0.4f).setEasePunch();

        txt_scoreCurrent.text = score + " Points";

    }

    public void RefreshHightScore(int score)
    {
        GameObject g_txt = txt_scoreHight.gameObject;

        LeanTween.cancel(g_txt);

        LeanTween.scale(g_txt, testScale, 0.6f).setEasePunch();

        txt_scoreHight.text = "HightPoints: " + score;
    }
}
