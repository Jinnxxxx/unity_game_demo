/// <summary>
/// 查找装备使用Dictionary的trygetvalue为尽量使用O(1)时间复杂度
/// </summary>

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance; // 单例模式

    public List<ItemData> startingItems; // 初始装备列表

    public List<InventoryItem> equipment; // 装备物品列表（用来遍历）
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictrionary;

    public List<InventoryItem> inventory; // 背包物品列表
    public Dictionary<ItemData, InventoryItem> inventoryDictrionary;

    public List<InventoryItem> stash; // 储物箱物品列表
    public Dictionary<ItemData, InventoryItem> stashDictrionary;



    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent; // 背包Slots的parentTransform
    [SerializeField] private Transform stashSlotParent; // 储物Slots的parentTransform
    [SerializeField] private Transform equipmentSlotParent; // 装备Slots的parentTransform

    private UI_ItemSlot[] inventoryItemSlot; // 背包Slots的UI列表
    private UI_ItemSlot[] stashItemSlot; // 储物Slots的UI列表
    private UI_EquipmentSlot[] equipmentSlot; // 装备Slots的UI列表


    [Header("Items cooldown")]
    private float lastTimeUsedFlask; // 上次使用生命药水的时间


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); // 如果已经存在实例，销毁当前实例，避免重复
    }

    void Start()
    {
        inventory = new List<InventoryItem>(); // 初始化背包物品列表
        inventoryDictrionary = new Dictionary<ItemData, InventoryItem>(); // <ItemData, InventoryItem>的字典

        stash = new List<InventoryItem>(); // 初始化储物物品列表
        stashDictrionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>(); // 初始化装备物品列表
        equipmentDictrionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>(); // 初始化背包的Slot列表
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>(); // 初始化储物的Slot列表
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>(); // 初始化装备的Slot列表


        AddStartingItems(); // 初始化player的初始装备

        lastTimeUsedFlask = Mathf.NegativeInfinity; // self_fix:初始化上次使用生命药水的时间
    }

    // 添加初始装备到Inventory
    private void AddStartingItems()
    {
        for (int i = 0; i < startingItems.Count; i++)
        {
            AddItem(startingItems[i]);
        }
    }

    // 装备装备
    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment; // 将_item转换为ItemData_Equipment类型
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;

        // 遍历equipmentDictrionary，找到与newEquipment相同的装备，若存在则移除旧的装备，添加新的装备
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictrionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = item.Key;
        }

        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment); // 移除于装备列表和字典
            AddItem(oldEquipment); // 将被移除的装备放回背包中
        }

        equipment.Add(newItem); // 添加至装备List
        equipmentDictrionary.Add(newEquipment, newItem); // 添加至装备Dictionary
        newEquipment.AddModifiers(); // 实装装备属性

        RemoveItem(_item); // 移除背包中的旧装备

        UpdateSlotUI();
    }

    // 卸下装备从inventory
    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictrionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value); // 移除装备列表中的旧装备
            equipmentDictrionary.Remove(itemToRemove); // 移除equipmentDictrionary中的旧装备
            itemToRemove.RemoveModifiers(); // 移除装备属性
            UpdateSlotUI();
        }
    }

    // 更新所有Slot的UI（先清理再更新）
    private void UpdateSlotUI()
    {
        // 清除装备槽位UI(self_fix)
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            equipmentSlot[i].CleanUpSlot();
        }

        // 遍历所有装备槽位，若装备Dictionary有相应装备，则更新图标至装备UI
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictrionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].UpdateSlot(item.Value);
            }
        }

        // 清除物品slot
        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();
        }
        // 清除储物slot
        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].CleanUpSlot();
        }

        // 更新背包Slots的UI
        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]); // 遍历inventoryItems中所有物品，更新背包Slots中所有物品的UI的icon和数量
        }
        // 更新储物Slots的UI
        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]); // 遍历stashItems中所有物品，更新储物Slots中所有物品的UI的icon和数量
        }
    }

    // 添加物品(to inventory or stash)
    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment)
            AddToInventory(_item);
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);

        UpdateSlotUI(); // 更新所有Slot的UI
    }

    // 添加物品到stash
    private void AddToStash(ItemData _item)
    {
        if (stashDictrionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item); // new一个新的ItemData的InventoryData
            stash.Add(newItem);
            stashDictrionary.Add(_item, newItem);
        }
    }

    // 添加物品到inventory
    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictrionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item); // new一个新的ItemData的InventoryData
            inventory.Add(newItem);
            inventoryDictrionary.Add(_item, newItem);
        }
    }

    // 移除物品，若stack<=1则移除物品，否则stack--
    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictrionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictrionary.Remove(_item);
            }
            else
                value.RemoveStack();
        }

        if (stashDictrionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictrionary.Remove(_item);
            }
            else
                stashValue.RemoveStack();
        }

        UpdateSlotUI(); // 更新所有Slot的UI
    }

    // 判断是否可以 craft
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        // 遍历requiredMaterials，若stash中没有足够的材料，则返回false
        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictrionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("not enough materials to craft this item");
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                Debug.Log("not enough materials to craft this item");
                return false;
            }
        }

        // 若材料足够，则移除inventory中的材料
        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data);
        }

        AddItem(_itemToCraft);
        Debug.Log("here is your item " + _itemToCraft.name);
        return true;
    }

    // 获得当前装备列表
    public List<InventoryItem> GetEquipmentList() => equipment;

    // 获得当前储物列表
    public List<InventoryItem> GetStashList() => stash;

    // 获取指定type装备数据
    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        // 遍历equipmentDictrionary，找到_type的装备，若存在则返回装备数据
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictrionary)
        {
            if (item.Key.equipmentType == _type)
                equipedItem = item.Key;
        }

        return equipedItem; // 返回已装备物品数据
    }

    // 使用药水
    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return; // 若没有生命药水则返回

        bool canUseFlask = Time.time > lastTimeUsedFlask + currentFlask.itemCooldown;

        if (canUseFlask)
        {
            currentFlask.Effect(null);
            lastTimeUsedFlask = Time.time; // 更新上次使用时间
        }
        else
        {
            Debug.Log("Flask is on cooldown");
        }

    }

}
