using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractTipPlane : UIBasePanel
{

    private Image img_tipLayouts;
    private Image img_Display;

    private TMP_Text txt_Tips;
    private TMP_Text txt_Operate;

    protected override void InitPanel()
    {
        base.InitPanel();

        img_Display = GetControl<Image>("img_Display");
        img_tipLayouts = GetControl<Image>("img_tipLayouts");

        txt_Operate = GetControl<TMP_Text>("txt_Operate");
        txt_Tips = GetControl<TMP_Text>("txt_Tips");

        EventCenter.GetInstance().AddEventListener<string>("Display_InteractTip", (desc) =>
        {
            if (desc.Length <= 0)
            {
                DisplayPlane(false);
                txt_Tips.text = "";
                return;
            }

            DisplayPlane(true);
            txt_Tips.text = desc;

        });

        DisplayPlane(false);
    }


}
