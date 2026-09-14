using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity; // 实体类
    private CharacterStats myStats; // 角色属性
    private RectTransform myTransform; // recttransform
    private Slider slider; // 滑块

    void Start()
    {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();


        entity.onFlipped += FlipUI; // 订阅翻转时事件
        myStats.onHealthChanged += UpdateHealthUI; // 订阅生命发生改变时事件

        UpdateHealthUI(); // 初始化血条UI
    }

    // Update UI event
    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue(); // 获取最大生命值
        slider.value = myStats.currentHealth; // 获取当前生命值
    }

    // Flip UI event
    private void FlipUI() => myTransform.Rotate(0, 180, 0);

    // unsubscribe all the events
    private void Ondisabled()
    {
        entity.onFlipped -= FlipUI;
        myStats.onHealthChanged -= UpdateHealthUI;
    }

}
