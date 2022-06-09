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
    bool invincible; //�Ƿ��޵�״̬

    public bool IsDied { get => dead; }

    // �Զ���Ѫ
    public virtual void ReCoverHealth()
    {
        invincible = (maxHealth <= 0);
        health = maxHealth;
        dead = false;
    }

    // �˺��ж�
    public virtual void GetDamage(float dmg)
    {
        if (invincible)
            return ;

        health = Mathf.Clamp(health - dmg, 0, maxHealth);

        // ȷ������
        if (!dead && health <= 0)
            dead = true;

    }


}
