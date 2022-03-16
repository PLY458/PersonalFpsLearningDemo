using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : UIBasePanel
{
    private List<TabButten> tabButtens;

    private TabButten selectedTab;

    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;

    private void Start()
    {
        selectedTab = null;

        EventCenter.GetInstance().AddEventListener<TabButten>("TabEnter", OnTabEnter);
        EventCenter.GetInstance().AddEventListener<TabButten>("TabExit", OnTabExit);
        EventCenter.GetInstance().AddEventListener<TabButten>("TabSelected", OnTabselected);

        FindTapControl();
    }


    private void FindTapControl()
    {
        TabButten[] controls = GetComponentsInChildren<TabButten>();

        if (tabButtens == null)
        {
            tabButtens = new List<TabButten>();
        }

        foreach (var tab in controls) 
        {
            tabButtens.Add(tab);
        }
    }

    public void OnTabEnter(TabButten tab)
    {
        ResetTabs();

        if (selectedTab == null || tab != selectedTab)
            tab.TabSpriteSet = tabHover;
    }

    public void OnTabExit(TabButten tab)
    {
        ResetTabs();
    }

    public void OnTabselected(TabButten tab)
    {
        selectedTab = tab;
        ResetTabs();
        tab.TabSpriteSet = tabActive;
        int index = tab.transform.GetSiblingIndex();
        EventCenter.GetInstance().EventTrigger("SetPageShow", index);
    }

    private void ResetTabs()
    {
        foreach (TabButten tab in tabButtens)
        {
            if (selectedTab != null && tab == selectedTab)
                continue;
            tab.TabSpriteSet = tabIdle;
        }
    }

}
