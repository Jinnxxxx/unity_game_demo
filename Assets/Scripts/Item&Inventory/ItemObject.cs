using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb; // 刚体组件
    [SerializeField] private ItemData itemData; // 关联的ItemData


    private void SetupVisuals()
    {
        // 空值检查，防报错
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.icon; // 设置精灵图标
        gameObject.name = "Item object - " + itemData.itemName; // 设置GameObject名称为ItemData的itemName
    }

    // 初始化物品数据和初始速度
    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;

        SetupVisuals(); // 初始化物品图标和名字
    }

    // 将ItemData添加到玩家的Inventory中
    public void PickupItem()
    {
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }

}
