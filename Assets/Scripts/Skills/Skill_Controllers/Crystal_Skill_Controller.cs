using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>(); // 获取动画组件
    private CircleCollider2D cd => GetComponent<CircleCollider2D>(); // 获取圆形碰撞器组件

    private float crystalExitTimer; // 水晶存在时间


    private bool canExplode; // 水晶是否爆炸
    private bool canMove; // 水晶是否可以移动
    private float moveSpeed; // 移动速度

    private bool canGrow; // 水晶是否变大
    private float growSpeed = 5; // 变大速度

    private Transform closestTarget; // 最近的目标
    [SerializeField] private LayerMask whatIsEnemy; // 敌人层掩码

    // 水晶参数初始化
    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestTarget)
    {
        crystalExitTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }


    //在范围内随机选择敌人
    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius(); //从（Blackhole_Skill）获取黑洞半径

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy); //过滤为Enemy层的collider

        if (colliders.Length > 0)
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
    }


    void Update()
    {
        crystalExitTimer -= Time.deltaTime;

        if (crystalExitTimer < 0)
            FinishCrystal(); //持续时间结束，触发爆炸或自毁

        //水晶移动
        if (canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime); // 逐帧移动到目标位置

            if (Vector2.Distance(transform.position, closestTarget.position) < 1)
            {
                FinishCrystal();
                canMove = false; // 移动到目标后，停止移动
            }

            //水晶线性变大
            if (canGrow)
                transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        }
    }

    //水晶爆炸造成伤害（anim的event调用）
    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().DamageEffect();
        }
    }


    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true; // 若水晶可以爆炸，则先变大再爆炸
            anim.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    // 自毁水晶object
    public void SelfDestroy() => Destroy(gameObject);

}
