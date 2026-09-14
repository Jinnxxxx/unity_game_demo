using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image itemImage; // 物品图标
    [SerializeField] private TextMeshProUGUI itemText; // 物品数量

    public InventoryItem item;


    // 初始化item、物品图标和物品数量
    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;

        if (item != null)
        {
            itemImage.color = Color.white; // 恢复slot的image的透明度

            itemImage.sprite = item.data.icon; // 设置slot的image为item的icon

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString(); // show number of item in slot
            }
            else
            {
                itemText.text = ""; // clear text
            }
        }
    }

    // 清楚slot槽位
    public void CleanUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    // 鼠标点击装备可装备
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item.data.itemType == ItemType.Equipment)
            Inventory.instance.EquipItem(item.data);
    }

}
