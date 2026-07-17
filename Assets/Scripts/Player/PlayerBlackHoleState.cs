using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackHoleState : PlayerState
{
    private float flyTime = .4f; //发动黑洞技能时，player飞行时间
    private bool skillUsed; //确保技能只使用一次

    private float defaultGravity; //默认重力

    public PlayerBlackHoleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale; //保存默认重力

        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0f; //重力设置为0
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity; //恢复默认重力
        player.MakeTransparent(false); //恢复透明度
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 15);
        if (stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -.1f);

            if (!skillUsed)
            {
                // 调用黑洞技能的CanUseSkill(UseSkill())方法
                if (player.skill.blackhole.CanUseSkill())
                    skillUsed = true;
            }
        }

        // 检查黑洞技能是否已经结束,如果是则切换到空中状态
        if (player.skill.blackhole.SkillCompleted())
        {
            stateMachine.ChangeState(player.airState);
            return;
        }
    }
}
