using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{

    private Player player => GetComponentInParent<Player>();


    // 更改变量triggercalled = true
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>(); // 获得目标的stats

                if (_target != null)
                    player.stats.DoDamage(_target); // 计算并造成伤害（包含动画）

                // ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);
                // if (weaponData != null)
                //     weaponData.Effect(_target.transform); 

                // 调用inventory的GetEquipment方法获取装备类型为Weapon的装备，并执行其effect
                Inventory.instance.GetEquipment(EquipmentType.Weapon)?.Effect(_target.transform);
            }
        }
    }



    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}
