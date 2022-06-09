using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButten : UIBasePanel, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{

    private Image tabBackGround;

    public Sprite TabSpriteSet {  set => tabBackGround.sprite = value; }

    void Start()
    {
        //EventCenter.GetInstance().EventTrigger("TabSubscribe", this);
        tabBackGround = GetControl<Image>("img_background");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventCenter.GetInstance().EventTrigger("TabSelected", this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventCenter.GetInstance().EventTrigger("TabEnter", this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventCenter.GetInstance().EventTrigger("TabExit", this);
    }


}
