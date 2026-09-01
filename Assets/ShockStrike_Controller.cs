using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage; // 伤害

    private Animator anim;
    private bool triggered;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }


    // 初始化伤害值和targetStats
    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }


    void Update()
    {
        if (!targetStats)
            return; // 如果目标为空，返回

        if (triggered)
            return; // 避免多次触发


        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime); // 移动雷电到目标位置
        transform.right = transform.position - targetStats.transform.position; // 设置雷电的方向

        if (Vector2.Distance(transform.position, targetStats.transform.position) < .1f)
        {
            anim.transform.localPosition = new Vector3(0, .5f); // 到达时position上移为自然
            anim.transform.localRotation = Quaternion.identity; // 初始化旋转

            transform.localRotation = Quaternion.identity; // 初始化旋转
            transform.localScale = new Vector3(3, 3); // 缩放雷电大小


            Invoke("DamageAndSelfDestroy", .2f); // 延迟2s调用伤害（先有动画再有伤害）
            triggered = true;
            anim.SetTrigger("Hit"); // 触发动画

        }
    }

    // 造成伤害，销毁对象
    private void DamageAndSelfDestroy()
    {
        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }



}
