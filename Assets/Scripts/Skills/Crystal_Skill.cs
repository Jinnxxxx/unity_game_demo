using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration; //水晶的持续时间
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Explosive crystal")]
    [SerializeField] private bool canExplode; //水晶是否可以爆炸

    [Header("Moving crystal")]
    [SerializeField] private bool canMoveToEnemy; //水晶是否可以移动到敌人
    [SerializeField] private float moveSpeed; //水晶移动速度

    public override void UseSkill()
    {
        base.UseSkill();

        if (currentCrystal == null)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity); //生成（实例化）水晶在玩家当前位置
            Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>(); //获取水晶的脚本

            currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform)); //设置水晶的持续时间,是否可以爆炸,是否可以移动到敌人,移动速度,最近的敌人
        }
        else
        {
            if (canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position; //获取玩家当前位置

            player.transform.position = currentCrystal.transform.position; //将玩家位置移动到水晶当前位置

            currentCrystal.transform.position = playerPos; //将水晶位置移动到玩家当前位置

            currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
        }
    }
}
