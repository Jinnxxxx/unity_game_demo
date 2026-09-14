using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemData itemData; // 关联的ItemData

    // 编辑器验证时
    void OnValidate()
    {
        GetComponent<SpriteRenderer>().sprite = itemData.icon; // 设置精灵图标
        gameObject.name = "Item object - " + itemData.itemName; // 设置GameObject名称为ItemData的itemName
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            // collider触碰到时，将ItemData添加到玩家的Inventory中
            Inventory.instance.AddItem(itemData);
            Destroy(gameObject);
        }
    }


}
