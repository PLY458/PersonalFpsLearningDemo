using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankCell : UIBasePanel
{
    // 排名信息
    private Image img_IconTop;
    private Image img_Trophy;
    private TMP_Text txt_Ranking;
    private TMP_Text txt_Score;

    // 玩家信息
    private Image img_Character;
    private Image img_Frame;
    private Image img_Nation;
    private TMP_Text txt_UserName;


    protected override void InitPanel()
    {
        base.InitPanel();

        img_Character = GetControl<Image>("img_Character");
        img_Frame = GetControl<Image>("img_Frame");
        img_Nation = GetControl<Image>("img_Nation");

        img_IconTop = GetControl<Image>("img_IconTop");
        img_Trophy = GetControl<Image>("img_Trophy");

        txt_Ranking = GetControl<TMP_Text>("txt_Ranking");
        txt_Score = GetControl<TMP_Text>("txt_Score");
        txt_UserName = GetControl<TMP_Text>("txt_UserName");

    }

    public void SetCellPanel(int pos, string name, int score)
    {
        txt_Ranking.text = pos.ToString();
        txt_Score.text = score.ToString();
        txt_UserName.text = name;
    }
}
