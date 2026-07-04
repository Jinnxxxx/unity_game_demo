using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>(); // 获取动画组件
    private CircleCollider2D cd => GetComponent<CircleCollider2D>(); // 获取圆形碰撞器组件

    private float crystalExitTimer; // 水晶存在时间


    private bool canExplode;
    private bool canMove;
    private float moveSpeed;


    // 水晶参数初始化
    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed)
    {
        crystalExitTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
    }


    void Update()
    {
        crystalExitTimer -= Time.deltaTime;

        if (crystalExitTimer < 0)
            FinishCrystal(); //持续时间结束，触发爆炸或自毁
    }

    //水晶爆炸造成伤害（anim的event调用）
    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().Damage();
        }
    }


    public void FinishCrystal()
    {
        if (canExplode)
            anim.SetTrigger("Explode");
        else
            SelfDestroy();
    }

    // 自毁水晶object
    public void SelfDestroy() => Destroy(gameObject);

}
