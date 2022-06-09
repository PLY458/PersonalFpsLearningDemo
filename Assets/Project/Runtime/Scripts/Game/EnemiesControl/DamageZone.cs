using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DamageZone : MonoBehaviour
{
    public E_DamageZone damageZone;

    DamageObject damgeObj;

    public DamageObject DamgeObj { get => damgeObj; }

    private void Start()
    {
        damgeObj = GetComponentInParent<DamageObject>();
    }

    public bool IsDamageObjDied()
    {
        if (damgeObj == null)
            return false;
        return damgeObj.IsDied;
    }


}
