using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Blackhole_Skill : Skill
{
    [SerializeField] private int amountOfAttacks; //克隆攻击的次数
    [SerializeField] private float cloneCooldown; //克隆攻击的间隔时间
    [SerializeField] private float blackholeDuration; //黑洞持续时间
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    Blackhole_Skill_Controller currentBlackhole;

    public override bool CanUseSkill()
    {
        return base.CanUseSkill(); //检查基础技能是否可以使用，若可以使用则返回true且调用UseSkill方法，否则返回false
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity); //创建黑洞实例

        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>(); //获取黑洞控制器组件

        currentBlackhole.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown, blackholeDuration); //初始化黑洞控制器
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    //检查黑洞技能是否完成
    public bool SkillCompleted()
    {
        if (!currentBlackhole)
            return false; //如果黑洞控制器组件为空，返回false

        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null; //清除黑洞控制器组件
            return true; //如果玩家已经退出黑洞状态，返回true
        }

        return false; //否则返回false
    }
}
