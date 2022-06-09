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
    [Header("�������/����Ŀ�趨")]
    [Range(1, 50)] public int rows;
    [Range(1, 50)] public int columns;

    [Header("����Ԫ�صĳߴ�ͼ������")]
    [SerializeField]
    private Vector2 cellSize, spacing;

    [Header("����Ԫ�����й���ͳߴ��޶�")]
    [SerializeField]
    private GridFitType fitType;
    [SerializeField]
    private bool fitX = true, fitY = true;
    
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (fitType <= GridFitType.Height)
        {
            // ���㵱ǰ�����������µ���Ҫ����/����(�ڲ���������ʽ��������)
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
            // �޶�����
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);

        }
        if (fitType == GridFitType.Height || fitType == GridFitType.FixedRows)
        {
            if(rows > transform.childCount)
            {
                rows = transform.childCount;
            }
            // �޶�����
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

        // ���ݸ������С�����������趨�ı߽��Ԫ��֮��Ĵ�С��
        float cellWidth = parentWidth / (float)columns - ((spacing.x / (float)columns) * 2)
            - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = parentHeight / (float)rows - ((spacing.y / (float)rows) * 2)
            - (padding.top / (float)rows) - (padding.bottom / (float)rows);

        // ��Ԫ�صĸ߿�����ѡ��
        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        // ����趨��Ԫ�ص�λ�úͿ������
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
