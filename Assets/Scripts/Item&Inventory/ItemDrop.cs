using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private GameObject dropPrefab; // 死亡时掉落物品的Prefab
    [SerializeField] private ItemData item; // 死亡时掉落物品的ItemData

    // 实例化掉落物品Prefab，并调用SetupItem函数设置ItemData和掉落速度
    public void DropItem()
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));

        newDrop.GetComponent<ItemObject>().SetupItem(item, randomVelocity);
    }

}