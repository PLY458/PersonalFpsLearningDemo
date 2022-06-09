using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageGroup : UIBasePanel
{
    [SerializeField]
    private List<UIBasePanel> pageControls;

    private void Start()
    {
        EventCenter.GetInstance().AddEventListener<int>("SetPageShow",DisplayerTargetPage);
    }

    private void DisplayerTargetPage(int index)
    {
        for (int i = 0; i < pageControls.Count; i++)
        {
            if (i == index)
            {
                pageControls[i].gameObject.SetActive(true);
            }
            else
            {
                pageControls[i].gameObject.SetActive(false);
            }

        }

    }


}
