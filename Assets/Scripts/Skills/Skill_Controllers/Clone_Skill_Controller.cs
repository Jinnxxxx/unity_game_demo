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

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        //克隆体透明度降低，消失
        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

            if (sr.color.a <= 0)
                Destroy(gameObject);
        }
    }

    // 初始化克隆体（克隆体的初始位置，克隆体的持续时间，克隆体是否可以攻击,克隆体的偏移量）
    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closestEnemy)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 4));

        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        closestEnemy = _closestEnemy;
        FaceClosestTarget();
    }


    private void AnimationTrigger()
    {
        cloneTimer = -.1f;

    }

    // 攻击触发器（anim添加event调用）
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);


        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().Damage();
        }
    }


    // 克隆体攻击时方向
    private void FaceClosestTarget()
    {
        // sprite默认朝左，如果最近敌人（closestEnemy）在左侧，sprite旋转180度
        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
                transform.Rotate(0, 180, 0);
        }
    }

}
