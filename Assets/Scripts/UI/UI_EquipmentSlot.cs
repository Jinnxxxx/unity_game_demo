using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;


    void OnValidate()
    {
        gameObject.name = "Equipment slot - " + slotType.ToString(); // 设置gameobject名
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        Inventory.instance.UnequipItem(item.data as ItemData_Equipment); // 从装备列表、装备字典中移除该装备，并移除player增加的数值
        Inventory.instance.AddItem(item.data as ItemData_Equipment); // 将卸下的装备添加回物品栏
        CleanUpSlot();
    }
}
