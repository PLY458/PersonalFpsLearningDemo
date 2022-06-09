using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_DamageZone
{
    Body, Head
}

public class TargetController : SingletonMono<TargetController>
{
    [SerializeField]
    List<TargetDamageObj> targetList;

    [SerializeField]
    bool autoHeal;

    [Header("∑÷ ˝…Ë÷√")]
    [SerializeField] int BodyPoint;
    [SerializeField] int HeadPoint;

    TargetDamageObj tempTarget;

    public void Start()
    {
        if (targetList != null)
        {
            foreach (var target in targetList)
            {
                target.InitTarget();
            }
        }
    }


    public void SetDamage( DamageZone zone, float damage, float headmult)
    {
        tempTarget = zone.DamgeObj as TargetDamageObj;

        if (tempTarget != null)
        {
            if (zone.damageZone == E_DamageZone.Head)
            {
                tempTarget.GetDamage(damage * headmult);
            }
            else
            {
                tempTarget.GetDamage(damage);
            }
        }

    }
}
