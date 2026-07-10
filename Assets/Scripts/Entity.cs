using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    //组件
    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    public SpriteRenderer sr { get; private set; }
    #endregion

    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;

    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    //地面检测
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    //墙体检测
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    //地面层
    [SerializeField] protected LayerMask whatIsGround;

    //角色方向
    public int facingDir { get; private set; } = 1; // 1: 右  -1: 左
    protected bool facingRight = true;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {

    }

    //伤害函数
    public virtual void Damage()
    {
        fx.StartCoroutine("FlashFX"); //受击闪烁
        StartCoroutine("HitKnockback"); //受击位移

        // Debug.Log(gameObject.name + "   was damaged");
    }

    //受击位移
    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        rb.velocity = new Vector2(knockbackDirection.x * -facingDir, knockbackDirection.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }

    #region Velocity
    //速度归0
    public void SetZeroVelocity()
    {
        //受击时
        if (isKnocked)
            return;

        rb.velocity = Vector2.zero;
    }

    //移动函数(设置速度)
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        //受击时
        if (isKnocked)
            return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region Collision
    //地面检测
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    //墙体检测
    public virtual bool IsWalldetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    //可视化辅助图形
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public virtual void FlipController(float _x)
    {
        if (Mathf.Abs(_x) < 0.05f)   // 阈值
            return;
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();

        // Debug.Log($"速度: {_x}, 面向右: {facingRight}");
    }
    #endregion

    //精灵透明度控制
    public void MakeTransparent(bool _transparent)
    {
        if (_transparent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }

}
