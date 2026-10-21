using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop; // 可掉落物品数量
    [SerializeField] private ItemData[] possibleDrop; // 可能掉落物品数组
    private List<ItemData> dropList = new List<ItemData>(); // 实际掉落物品列表

    [SerializeField] private GameObject dropPrefab; // 死亡时掉落物品的空Prefab


    public virtual void GenerateDrop()
    {
        // 可掉落物品list
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrop[i].dropChance)
                dropList.Add(possibleDrop[i]);
        }

        // 实际掉落物品
        for (int i = 0; i < possibleItemDrop; i++)
        {
            if (dropList.Count == 0) break; // 如果掉落列表为空，跳出循环

            ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];

            dropList.Remove(randomItem); // 从掉落列表中移除已掉落的物品，避免重复掉落
            DropItem(randomItem); // 实例化Prefab，初始化物品数据
        }
    }


    // 实例化掉落物品Prefab，并调用SetupItem函数设置ItemData和掉落速度
    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }

}