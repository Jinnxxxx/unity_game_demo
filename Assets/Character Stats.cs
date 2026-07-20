using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    public Stat strength; //角色力量值
    public Stat damage; //角色伤害值
    public Stat maxHealth; //角色最大生命值

    [SerializeField] private int currentHealth; //角色当前生命值



    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue(); //初始化当前生命值为最大生命值
    }


    //计算并造成伤害
    public virtual void DoDamage(CharacterStats _targetStats)
    {


        int totalDamage = damage.GetValue() + strength.GetValue(); //计算总伤害值

        _targetStats.TakeDamage(totalDamage); //执行目标角色受到伤害
    }

    //执行造成伤害
    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage; //减少当前生命值

        Debug.Log(name + " took damage : " + _damage);

        if (currentHealth < 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        //throw new NotImplementedException();
    }

}
