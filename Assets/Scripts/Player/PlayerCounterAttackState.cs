using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{

    private bool canCreateClone; //限制克隆创建数量

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;
        stateTimer = player.counterAttackDuration; //反击持续时间
        player.anim.SetBool("SuccessfulCounterAttack", false); //重置反击成功动画
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned()) //反击成功
                {
                    stateTimer = 10; //防止过早退出
                    player.anim.SetBool("SuccessfulCounterAttack", true); //进入反击成功动画

                    if (canCreateClone)
                    {
                        canCreateClone = false; //克隆创建数量限制
                        player.skill.clone.CreateCloneOnCounterAttack(hit.transform); //反击成功后创建克隆}
                    }
                }
            }

            if (stateTimer < 0 || triggerCalled)
                stateMachine.ChangeState(player.idleState);
        }
    }
}
