using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour
{

    private SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material orignalMat;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        orignalMat = sr.material;
    }

    //受击闪烁
    private IEnumerator FlashFX()
    {
        sr.material = hitMat;

        yield return new WaitForSeconds(flashDuration);

        sr.material = orignalMat;
    }

    //弹反成功红色闪烁
    private void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    //弹反成功结束后恢复颜色
    private void CancelRedBlink()
    {
        CancelInvoke();
        sr.color = Color.white;
    }

}
