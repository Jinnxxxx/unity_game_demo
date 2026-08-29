using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour
{

    private SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private float flashDuration; // 材质变换间隔
    [SerializeField] private Material hitMat; // 变换材质
    private Material orignalMat; // 原材质

    [Header("Ailment colors")]
    [SerializeField] private Color[] igniteColor; // 点燃颜色
    [SerializeField] private Color[] chillColor; // 冰冻颜色
    [SerializeField] private Color[] shockColor; // 雷击颜色

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        orignalMat = sr.material; // 保存原材质
    }

    // 受击闪烁
    private IEnumerator FlashFX()
    {
        sr.material = hitMat; // 变换材质
        Color currentColor = sr.color; // 保存当前颜色
        sr.color = Color.white; // 变为白色

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor; // 恢复原颜色
        sr.material = orignalMat; // 恢复原材质
    }

    // 弹反成功红色闪烁
    private void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    // (弹反成功结束后)恢复颜色
    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;
    }

    // 触发点燃特效
    public void IgniteFxFor(float _seconds)
    {
        InvokeRepeating("IgniteColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    // 点燃特效
    private void IgniteColorFx()
    {
        // 两种颜色切换
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    // 触发冰冻特效
    public void ChillFxFor(float _seconds)
    {
        InvokeRepeating("ChillColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    // 冰冻特效
    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }

    // 触发电击特效
    public void ShockFxFor(float _seconds)
    {
        InvokeRepeating("ShockColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    // 电击特效
    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }



}
