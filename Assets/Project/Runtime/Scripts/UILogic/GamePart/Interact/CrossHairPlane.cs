using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairPlane : UIBasePanel
{

    Image img_Cursor;
    Image img_RightSide;
    Image img_LeftSide;
    Image img_TopSide;
    Image img_BottomSide;
    Image img_HitMark;

    Image img_HealthBarFrame;
    Image img_fillBK;
    Image img_fill;

    float spread = 0.01f;
    float showingHit = 0;
    float showingHealthBar = 0;

    Color fadedColor;

    DamageObject tempObj;

    RectTransform planeRect;

    protected override void InitPanel()
    {
        base.InitPanel();

        img_Cursor = GetControl<Image>("img_Cursor");
        img_RightSide = GetControl<Image>("img_RightSide");
        img_LeftSide = GetControl<Image>("img_LeftSide");
        img_TopSide = GetControl<Image>("img_TopSide");
        img_BottomSide = GetControl<Image>("img_BottomSide");
        img_HitMark = GetControl<Image>("img_HitMark");
        img_HitMark.color = new Color(1f, 1f, 1f, 0f);

        img_HealthBarFrame = GetControl<Image>("img_HealthBarFrame");
        img_fillBK = GetControl<Image>("img_fillBK");
        img_fill = GetControl<Image>("img_fill");
        

        fadedColor = Color.white;

        planeRect = GetComponent<RectTransform>();

        EventCenter.GetInstance().AddEventListener<float>("SetCrossHair_crossHair",(sp)=> { spread = sp; });
        EventCenter.GetInstance().AddEventListener<bool>("SetHitMarker_crossHair", (iskilled) =>
        {
            img_HitMark.color = (iskilled) ? Color.red : Color.white;
            showingHit = 0.5f;
        });
        EventCenter.GetInstance().AddEventListener<DamageObject>("SetHealthBar_crossHair", (Dobj) =>
        {

            tempObj = Dobj;
            Color barColor = img_fillBK.color;
            barColor.a = 1f;
            img_fillBK.color = barColor;
            showingHealthBar = 0.2f;
            
        });

    }


    public override void RefreshPlane()
    {
        base.RefreshPlane();
        UpdateCrosshair();
    }


    public void UpdateCrosshair()
    {
        UpdateSpread();
        UpdateFade();
        UpdateHitMark();
        //UpdateHealthBar();
    }

    void UpdateFade()
    {
        int dir = (spread <= 0.01f) ? -1 : 1;
        fadedColor.a = Mathf.Clamp(fadedColor.a + (dir * Time.deltaTime * 2), 0.0f, 1.0f);

        img_Cursor.color = fadedColor;
        img_TopSide.color = fadedColor;
        img_BottomSide.color = fadedColor;
        img_LeftSide.color = fadedColor;
        img_RightSide.color = fadedColor;
    }

    void UpdateSpread()
    {
        //1019.6x + 12
        //crosshair equation i got from plotting a bunch of spreads and their respective crosshair distances
        int crosshairDis = (int)(1019.6f * spread * 1.8f) + 12;
        Vector2 xPos = new Vector2(crosshairDis‬, 0);
        Vector2 yPos = new Vector2(0, crosshairDis‬);
        img_TopSide.rectTransform.anchoredPosition = Vector2.Lerp(img_TopSide.rectTransform.anchoredPosition, yPos, Time.deltaTime * 8f);
        img_BottomSide.rectTransform.anchoredPosition = Vector2.Lerp(img_BottomSide.rectTransform.anchoredPosition, -yPos, Time.deltaTime * 8f);
        img_LeftSide.rectTransform.anchoredPosition = Vector2.Lerp(img_LeftSide.rectTransform.anchoredPosition, xPos, Time.deltaTime * 8f);
        img_RightSide.rectTransform.anchoredPosition = Vector2.Lerp(img_RightSide.rectTransform.anchoredPosition, -xPos, Time.deltaTime * 8f);
    }

    void UpdateHitMark()
    {
        if (showingHit <= 0)
        {
            Color hitColor = img_HitMark.color;
            hitColor.a = Mathf.Lerp(hitColor.a, 0, Time.deltaTime * 16f);
            img_HitMark.color = hitColor;
        }
        else
            showingHit -= Time.deltaTime;
    }

    void UpdateHealthBar()
    {
        if (showingHealthBar > 0)
        {
            if (tempObj != null)
            {
                Vector3 endPos = Camera.main.WorldToScreenPoint(tempObj.transform.position);
                Vector2 temp = img_HealthBarFrame.rectTransform.localPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    planeRect, endPos, Camera.main, out temp);
                img_HealthBarFrame.rectTransform.localPosition = temp;

                Debug.Log("锁定UI位置：" + temp.ToString());
            }

            showingHealthBar -= Time.deltaTime;
        }
        else
        {
            Color barColor = img_fillBK.color;
            barColor.a = Mathf.Lerp(barColor.a, 0, Time.deltaTime * 16f);
            img_fillBK.color = barColor;
        }
         
    }
}
