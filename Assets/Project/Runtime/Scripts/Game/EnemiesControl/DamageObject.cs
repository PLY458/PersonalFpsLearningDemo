using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageObject : MonoBehaviour
{
    public float maxHealth = 100f;
    public float uiHeight = 5f;
    protected float health;

    bool dead;
    bool invincible; //是否无敌状态

    public bool IsDied { get => dead; }

    // 自动回血
    public virtual void ReCoverHealth()
    {
        invincible = (maxHealth <= 0);
        health = maxHealth;
        dead = false;
    }

    // 伤害判定
    public virtual void GetDamage(float dmg)
    {
        if (invincible)
            return ;

        health = Mathf.Clamp(health - dmg, 0, maxHealth);

        // 确认死亡
        if (!dead && health <= 0)
            dead = true;

    }


}
