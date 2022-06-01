using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankPlane : UIBasePanel
{
    private Image img_Curtain;

    private Button btn_GetPlayerLeaderBoard;
    private Button btn_SetRandomPlayerData;

    private Button btn_Exit;
    private TMP_Text txt_Tips;
    private GameObject tipsObj;

    private CanvasGroup _curtainAlpha;

    private RankCell tempRankCell;
    private List<RankCell> rankCellList;

    [SerializeField]
    private Transform _rankListFrame;
    [SerializeField]
    private Transform _rankListContent;

    Vector3 testScale = new Vector3(1.3f, 1.3f, 0);

    protected override void InitPanel()
    {
        base.InitPanel();
        txt_Tips = GetControl<TMP_Text>("txt_Tips");
        tipsObj = txt_Tips.gameObject;

        img_Curtain = GetControl<Image>("img_Curtain");

        btn_GetPlayerLeaderBoard = GetControl<Button>("btn_GetPlayerLeaderBoard");
        btn_SetRandomPlayerData = GetControl<Button>("btn_SetRandomPlayerData");
        btn_Exit = GetControl<Button>("btn_Exit");

        _curtainAlpha = img_Curtain.gameObject.GetComponent<CanvasGroup>();

        btn_Exit.onClick.AddListener(ExitDialog);
        btn_GetPlayerLeaderBoard.onClick.AddListener(GetLeaderBoard);

        EventCenter.GetInstance().AddEventListener<bool>("Display_RankListPlane", DisplayPlane);
        EventCenter.GetInstance().AddEventListener("Undisplay_RankListPlane", ExitDialog);
        EventCenter.GetInstance().AddEventListener("GenerateRankCell", GenerateRankCell);

        DisplayPlane(false);
    }

    private void GetLeaderBoard()
    {
        PlayFabMgr.GetInstance().GetLeaderboard();
    }

    // TODO 设置生成事件，在PlayFabMgr回调中触发

    private void GenerateRankCell()
    {
        if (rankCellList == null)
        {
            rankCellList = new List<RankCell>();
        }

        if (PlayFabMgr.GetInstance().PlayRankList.Count > 0)
        {
            // 清除记录中的物体
            foreach (var cell in rankCellList)
            {
                Destroy(cell.gameObject);
            }

            rankCellList.Clear();

            foreach (var player in PlayFabMgr.GetInstance().PlayRankList)
            {

                ResourcesMgr.GetInstance().LoadAsync<GameObject>("UI/GamePart/RankSystem/RankCell",
                (obj) => {
                    obj.transform.SetParent(_rankListContent);

                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                

                    (obj.transform as RectTransform).offsetMax = new Vector2(1, 160);
                    (obj.transform as RectTransform).offsetMin = Vector2.one;

                    //得到预设体身上的面板脚步
                    tempRankCell = obj.GetComponent<RankCell>();

                    tempRankCell.SetCellPanel(player.indexPos + 1, player.playName, player.playScore);

                    rankCellList.Add(tempRankCell);


                    
                });


            }

            txt_Tips.text = "排行榜更新完毕！";

            LeanTween.cancel(tipsObj);
            LeanTween.scale(tipsObj, testScale, 0.4f).setEasePunch();

            
            //if (tempRankCell == null)
            //{
            //    tempRankCell = new RankCell();
            //}

        }
    }


    private void OnEnable()
    {
        _curtainAlpha.alpha = 0;
        _curtainAlpha.LeanAlpha(1, 0.6f);

        _rankListFrame.localPosition = new Vector2(-Screen.width, 0);
        _rankListFrame.LeanMoveLocalX(0, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    private void ExitDialog()
    {
        _curtainAlpha.LeanAlpha(0, 0.6f);
        _rankListFrame.LeanMoveLocalX(-Screen.width, 0.5f)
            .setEaseInExpo().setOnComplete(() => DisplayPlane(false));
    }



}
