using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    [SerializeField] private float returnSpeed = 12;
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    [Header("Bounce info")]
    [SerializeField] private float bounceSpeed;
    private bool isBouncing;
    private int amountOfBounce;
    private List<Transform> enemyTarget;
    private int targetIndex;


    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    // 初始化剑的状态
    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player)
    {
        player = _player;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        anim.SetBool("Rotation", true);
    }

    // 初始化弹跳剑
    public void SetupBounce(bool _isBouncing, int _amountOfBounces)
    {
        isBouncing = _isBouncing;
        amountOfBounce = _amountOfBounces;
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

        // 弹跳剑
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                targetIndex++;
                amountOfBounce--;
                // 控制bounce次数
                if (amountOfBounce <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
                // 防止index越界
                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }


    }

    // 触碰后，停止行为
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果剑正在返回，不处理碰撞事件
        if (isReturning)
            return;

        // 触碰到敌人，初始化周围敌人list
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

        StuckInto(collision);

    }


    //剑卡在敌人身上
    private void StuckInto(Collider2D collision)
    {

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