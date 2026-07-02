using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;
    private float returnSpeed = 12;

    // 穿刺剑信息
    [Header("Pierce info")]
    private float pierceAmount;


    // 弹跳剑信息
    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;


    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    // 初始化剑的状态
    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        if (pierceAmount <= 0)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 7);
    }

    // 初始化弹跳剑
    public void SetupBounce(bool _isBouncing, int _amountOfBounces, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();
    }

    // 初始化穿刺剑
    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    // 初始化旋转剑
    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    // 冻结axis,并设置isReturning为true
    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //(不影响)rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }

    void Update()
    {
        // 根据运动方向改变transform
        if (canRotate)
            transform.right = rb.velocity;

        // 剑返回
        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);
            // 如果剑与玩家的距离小于1，清除剑
            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchTheSword(); //改变state，destroy sword
        }

        // 弹跳剑逻辑
        BounceLogic();

        // 穿刺剑逻辑
        SpinLogic();
    }

    // 穿刺剑逻辑
    private void SpinLogic()
    {
        if (isSpinning)
        {
            // 如果剑与玩家的距离大于maxTravelDistance，停止旋转和移动
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                // 剑停下后，旋转同时以1.5f的速度朝前移动
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isReturning = true; //剑返回
                    isSpinning = false;
                }

                // 伤害触发
                hitTimer -= Time.deltaTime;
                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>());//造成伤害
                    }
                }

            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        spinTimer = spinDuration;
    }

    // 弹跳剑逻辑
    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime); // 剑朝敌人平滑移动

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                //返回时造成伤害
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                targetIndex++;
                // 防止index越界
                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;

                bounceAmount--;
                // 控制bounce次数
                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
            }
        }
    }

    // 触碰后，停止行为
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果剑正在返回，不处理碰撞事件
        if (isReturning)
            return;

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);//造成伤害
        }

        // 触碰到敌人，初始化周围敌人list
        SetupTargetForBounce(collision);

        StuckInto(collision);

    }

    private void SwordSkillDamage(Enemy enemy)
    {
        enemy.Damage();
        enemy.StartCoroutine("FreezeTimerFor", freezeTimeDuration);
    }

    private void SetupTargetForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }


    //剑卡在敌人身上（冻结并改为Kinematic）
    private void StuckInto(Collider2D collision)
    {
        // 若为穿刺剑，collide后继续运动      
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        // 若为旋转剑, collide后不进行stuck行为
        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        // 若不是前两者，则卡在敌人身上
        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)
        {
            return;
        }

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}