using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloatRange
{
    public float min;
    public float max;

    public FloatRange()
    {
        min = max = 0;
    }

    public FloatRange(float mn, float mx)
    {
        min = mn;
        max = mx;
    }

    public float GetDifference()
    {
        return (max - min);
    }
}
