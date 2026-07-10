using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skill.clone.CreateCloneOnDashStart(); //冲刺开始时创建clone

        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();

        player.skill.clone.CreateCloneOnDashOver(); //冲刺结束时创建clone

        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        //不在地面，在墙上的话切换到墙滑状态
        if (!player.IsGroundDetected() && player.IsWalldetected())
            stateMachine.ChangeState(player.wallSlide);

        //dash设置
        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }
}
