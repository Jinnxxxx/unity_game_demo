using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player; //获取player引用
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }


    // check if the skill can be used
    // if the skill can be used, use the skill and reset the cooldown timer
    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        Debug.Log("Skill is on cooldown");
        return false;
    }


    public virtual void UseSkill()
    {
        //do some skill spesific things！！！
    }

    // 寻找最近的敌人
    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        //Debug.Log(_checkTransform.position);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // 遍历所有敌人，找到最近敌人，计算距离
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }
        
        return closestEnemy; // 返回最近敌人
    }
}
