using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb; // 刚体组件
    [SerializeField] private ItemData itemData; // 关联的ItemData
    [SerializeField] private Vector2 velocity; // 物品掉落时初始速度

    // 编辑器验证时
    void OnValidate()
    {
        // 防报错
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.icon; // 设置精灵图标
        gameObject.name = "Item object - " + itemData.itemName; // 设置GameObject名称为ItemData的itemName
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            rb.velocity = velocity;
    }


    // 初始化物品数据和初始速度
    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;
    }

    // 将ItemData添加到玩家的Inventory中
    public void PickupItem()
    {
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }

}
