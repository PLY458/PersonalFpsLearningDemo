using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairPlane : UIBasePanel
{

    private Image img_Cursor;
    private Image img_RightSide;
    private Image img_LeftSide;
    private Image img_TopSide;
    private Image img_BottomSide;

    protected override void InitPanel()
    {
        base.InitPanel();

        img_Cursor = GetControl<Image>("img_Cursor");
        img_RightSide = GetControl<Image>("img_RightSide");
        img_LeftSide = GetControl<Image>("img_LeftSide");
        img_TopSide = GetControl<Image>("img_TopSide");
        img_BottomSide = GetControl<Image>("img_BottomSide");

        
    }

}
