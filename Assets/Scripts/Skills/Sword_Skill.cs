using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular; //剑的类型（默认为普通剑）

    // 弹跳剑参数
    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    // 穿刺剑参数
    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    // 旋转剑参数
    [Header("Spin info")]
    [SerializeField] private float hitCooldown = .35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;


    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    //发射剑最终方向
    private Vector2 finalDir;

    //瞄准点
    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBeetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;


    protected override void Start()
    {
        base.Start();

        GenerateDots(); //生成瞄准点（不显示）

        SetupGravity(); //设置不同剑类型的gravity
    }

    // 设置不同剑类型的gravity
    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        //松开右键后，计算最终方向
        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(
                AimDirection().normalized.x * launchForce.x,
                AimDirection().normalized.y * launchForce.y);

        //按住右键有，生成瞄准点
        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                //spaceBeetweenDots实际上为设置的时间差（匀加速运动公式中t）
                dots[i].transform.position = DotsPosition(i * spaceBeetweenDots);
            }
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation); //创建prefab实例
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>(); //获取Sword_Skill_Controller脚本


        //设置剑的类型(传参为后初始化准备)
        if (swordType == SwordType.Bounce)
            newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);


        //初始化剑
        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration,returnSpeed);

        //给Player的sword赋值
        player.AssignNewSword(newSword);

        //松开右键后，隐藏瞄准点
        DotsActivate(false);
    }


    #region Aim region
    //瞄准方向（鼠标位置 - 玩家位置）
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    //激活/关闭瞄准点
    public void DotsActivate(bool _activate)
    {
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i].SetActive(_activate);
        }
    }

    //生成瞄准点（未激活）
    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    //匀加速运动
    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y)
            * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }
    #endregion

}
