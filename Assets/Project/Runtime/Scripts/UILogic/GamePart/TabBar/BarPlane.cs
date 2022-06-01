using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarPlane : UIBasePanel
{
    private Image img_fillBK;

    private Image img_fill;

    private float current_Percent;

    private float minless_Percent;

    [SerializeField]
    private Color color_Enough;
    [SerializeField]
    private Color color_Warning;

    protected override void InitPanel()
    {
        base.InitPanel();
        img_fillBK = GetControl<Image>("img_fillBK");
        img_fill = GetControl<Image>("img_fill");
        EventCenter.GetInstance().AddEventListener<float>("SetStaminaCurrent", (percent) => { current_Percent = percent; });
        EventCenter.GetInstance().AddEventListener<float>("SetStaminaMinless", (percent) => { minless_Percent = percent; });

    }

    public override void RefreshPlane()
    {
        base.RefreshPlane();
        img_fill.fillAmount = Mathf.Lerp(img_fill.fillAmount, current_Percent, 3.0f * Time.deltaTime);

        if (current_Percent <= minless_Percent)
        {
            img_fill.color = color_Warning;
        }
        else
        {
            img_fill.color = color_Enough;
        }

    }



}
