using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration; //水晶的持续时间
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private bool cloneInsteadOfCrystal; //是否克隆代替水晶

    [Header("Explosive crystal")]
    [SerializeField] private bool canExplode; //水晶是否可以爆炸

    [Header("Moving crystal")]
    [SerializeField] private bool canMoveToEnemy; //水晶是否可以移动到敌人
    [SerializeField] private float moveSpeed; //水晶移动速度

    [Header("Multi stacking crystal")]
    [SerializeField] private bool canUseMultiStacks; //水晶是否可以堆叠
    [SerializeField] private int amountOfStacks; //水晶的堆叠数量
    [SerializeField] private float multiStackCooldown; //多水晶冷却时间
    [SerializeField] private float useTimeWindow; //多水晶使用窗口时间
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>(); //剩余的水晶列表

    private float originalCooldown; //保存原始冷却时间(self_fix)

    protected override void Start()
    {
        base.Start();
        
        RefillCrystall(); //填充多水晶到列表(self_fix)
        originalCooldown = cooldown; //保存原始冷却时间(self_fix)
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (!CanUseMultiCrystal())
            cooldown = originalCooldown; //若不能使用多水晶，则恢复原始冷却时间(self_fix)

        // 检查是否可以使用多水晶,若为真后面代码不工作
        if (CanUseMultiCrystal())
            return;



        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position; //获取玩家当前位置
            player.transform.position = currentCrystal.transform.position; //将玩家位置移动到水晶当前位置
            currentCrystal.transform.position = playerPos; //将水晶位置移动到玩家当前位置

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero); //生成克隆体
                Destroy(currentCrystal); //销毁水晶(不允许爆炸)
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity); //生成（实例化）水晶在玩家当前位置
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>(); //获取水晶的脚本

        //设置水晶的持续时间,是否可以爆炸,是否可以移动到敌人,移动速度,最近的敌人
        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform));


    }

    //选择范围内随即目标
    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();


    // 检查是否可以使用多水晶
    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            //respawn crystal
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks) //若使用的是第一个水晶，则进入时间窗口倒计时（延迟调用ResetAbility）
                    Invoke("ResetAbility", useTimeWindow);

                cooldown = 0; //有剩余水晶时，不进入冷却时间
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1]; //从列表中取出最后一个水晶
                GameObject newCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity); //生成（实例化）水晶在玩家当前位置

                crystalLeft.Remove(crystalToSpawn); //从列表中移除最后一个水晶

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform)); //设置水晶的持续时间,是否可以爆炸,是否可以移动到敌人,移动速度,最近的敌人

                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown; //没有剩余水晶时，进入冷却时间
                    RefillCrystall(); //填充多水晶到列表
                }

                return true;
            }
        }
        return false;
    }


    // 填充多水晶到列表
    private void RefillCrystall()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count; //计算需要添加的水晶数量

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }


    //进入冷却时间，重新填充水晶到列表
    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;

        cooldownTimer = multiStackCooldown;
        RefillCrystall();
    }
}
