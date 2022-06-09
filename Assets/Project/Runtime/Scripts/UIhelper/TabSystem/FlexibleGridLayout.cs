using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum GridFitType
{
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
}

public class FlexibleGridLayout : LayoutGroup
{
    [Header("网格的行/列数目设定")]
    [Range(1, 50)] public int rows;
    [Range(1, 50)] public int columns;

    [Header("网格元素的尺寸和间隔属性")]
    [SerializeField]
    private Vector2 cellSize, spacing;

    [Header("网格元素排列规则和尺寸限定")]
    [SerializeField]
    private GridFitType fitType;
    [SerializeField]
    private bool fitX = true, fitY = true;
    
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (fitType <= GridFitType.Height)
        {
            // 计算当前子物体数量下的需要的行/列数(在不对齐行列式的需求下)
            float sqrRt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        if (fitType == GridFitType.Width || fitType == GridFitType.FixedColumns)
        {
            if (columns > transform.childCount)
            {
                columns = transform.childCount;
            }
            // 限定行数
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);

        }
        if (fitType == GridFitType.Height || fitType == GridFitType.FixedRows)
        {
            if(rows > transform.childCount)
            {
                rows = transform.childCount;
            }
            // 限定列数
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);

        }

        if (spacing.x < 0)
        {
            spacing.x = 0;
        }
        if (spacing.y < 0)
        {
            spacing.y = 0;
        }

        // 根据父物体大小，行列数，设定的边界和元素之间的大小来
        float cellWidth = parentWidth / (float)columns - ((spacing.x / (float)columns) * 2)
            - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = parentHeight / (float)rows - ((spacing.y / (float)rows) * 2)
            - (padding.top / (float)rows) - (padding.bottom / (float)rows);

        // 子元素的高宽属性选择
        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        // 逐个设定子元素的位置和宽高属性
        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];

            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);

        }

    }

    public override void CalculateLayoutInputVertical()
    {

    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }

}
