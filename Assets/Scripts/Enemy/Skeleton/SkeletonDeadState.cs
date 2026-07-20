using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonDeadState : EnemyState
{
    private Enemy_Skeleton enemy;

    public SkeletonDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }


    public override void Enter()
    {
        base.Enter();

        enemy.anim.SetBool(enemy.lastAnimBoolName, true); //进入死亡状态后，立刻设置动画为死亡前一刻状态的动画
        enemy.anim.speed = 0; //死亡状态后，动画速度为0
        enemy.cd.enabled = false; //死亡状态后，禁用胶囊碰撞器

        stateTimer = .15f;
    }


    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 10); //死亡状态后，角色停止移动
    }
}
