using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingPlane : UIBasePanel
{
    private Image img_Curtain;
    private Image img_Frame;

    private TMP_Text txt_LoadValue;

    private Slider slid_Loading;

    private CanvasGroup _curtainAlpha;



    protected override void InitPanel()
    {
        base.InitPanel();

        img_Curtain = GetControl<Image>("img_Curtain");
        img_Frame = GetControl<Image>("img_Frame");

        txt_LoadValue = GetControl<TMP_Text>("txt_LoadValue");

        slid_Loading = GetControl<Slider>("slid_Loading");

        _curtainAlpha = img_Curtain.gameObject.GetComponent<CanvasGroup>();

        EventCenter.GetInstance().AddEventListener<bool>("Display_LoadingPlane", DisplayPlane);

        EventCenter.GetInstance().AddEventListener<float>("Repear_LoadingScene", RefeshLoadValue);

        txt_LoadValue.text = "0%";

        slid_Loading.value = 0f;

        DisplayPlane(false);
    }

    private void OnEnable()
    {
        _curtainAlpha.alpha = 0;
        _curtainAlpha.LeanAlpha(1, 0.6f);

        img_Frame.transform.localPosition = new Vector2(Screen.width, 0);
        img_Frame.transform.LeanMoveLocalX(0, 0.5f).setEaseOutExpo().delay = 0.2f;

        StartCoroutine(StartLoadingSceneDelay("GameScene"));
    }

    private IEnumerator StartLoadingSceneDelay(string name)
    {

        yield return new WaitForSeconds(0.5f);
        //Debug.Log("加载场景启动！！");
        ScenceMgr.GetInstance().LoadSceneAsyn(name,
            () => {
                //Debug.Log("场景加载完成: ");
                GameManager.GetInstance().InitGame();
            });

    }

    public override void RefreshPlane()
    {
        base.RefreshPlane();
    }

    private void RefeshLoadValue(float Progress)
    {

        int wholePercent = (int)(Progress * 100f);

        //Debug.Log("获取进度条数据：" + wholePercent.ToString());

        slid_Loading.value = Progress * 100f;

        txt_LoadValue.text = wholePercent.ToString() + "%";

    }
}
