using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDamageObj : DamageObject
{
    // 痊愈时间
    public float BackUpTime = 2f;
    //Animator ani;

    // 动画管理属性
    //float hurt = 0f;
    //float previousHealth;

    public void InitTarget()
    {
        //previousHealth = health;
        ReCoverHealth();
    }

    public void UpdateTarget()
    {
        //hurt = Mathf.Lerp(hurt, (IsDied) ? 1f : 0f, Time.deltaTime * 4f);
    }

    public float GetHealthPercent { get { return Mathf.Clamp( health / maxHealth   , 0f, 1f) ; } }
    //void HurtTarget()
    //{
    //    if (IsDied)
    //        return;
    //    float dmgDone = (previousHealth - health);
    //    hurt = dmgDone / (maxHealth * 2);
    //}

    //void TargetDied()
    //{
    //    StartCoroutine(getBackUp());
    //    IEnumerator getBackUp()
    //    {
    //        hurt = 1f;
    //        yield return new WaitForSeconds(BackUpTime);
    //        ReCoverHealth();
    //    }
    //}
}
