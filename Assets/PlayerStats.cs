using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }


    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage); // 减少血量（包括血条UI）

        player.DamageEffect(); // 受击动画
    }


    protected override void Die()
    {
        base.Die();

        player.Die(); // 切换状态机
    }
}
