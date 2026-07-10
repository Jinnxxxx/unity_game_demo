using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;

    private float cloneTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy; //最近敌人
    private int facingDir = 1; //默认为向右


    private bool canDuplicateClone; //允许再克隆
    private float chanceToDuplicate;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        // 持续时间结束后，克隆体透明度降低，消失
        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

            if (sr.color.a <= 0)
                Destroy(gameObject);
        }
    }

    // 初始化克隆体（克隆体的初始位置，克隆体的持续时间，克隆体是否可以攻击,克隆体的偏移量）
    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicate, float _chanceToDuplicate)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 4));

        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        //closestEnemy = _closestEnemy;
        closestEnemy = FindClosestEnemy(); //TEST

        FaceClosestTarget();
    }


    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }


    // 攻击触发器（anim添加event调用）
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius); //返回以自身为中心，半径为attackCheckRadius的圆形区域内的所有碰撞器

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Enemy>().Damage();


                // 若允许克隆技能再产生一个克隆体，有**概率再生产一个克隆体
                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }
        }

    }


    // 克隆体攻击时方向
    private void FaceClosestTarget()
    {
        // sprite默认朝左，如果最近敌人（closestEnemy）在左侧，sprite旋转180度
        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }

    }

    //TEST
    // 寻找最近的敌人
    private Transform FindClosestEnemy()
    {
        Debug.Log(transform.position);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // 遍历所有敌人，找到最近敌人，计算距离
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        return closestEnemy; // 返回最近敌人
    }

}
