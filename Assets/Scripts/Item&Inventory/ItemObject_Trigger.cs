using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject myItemObject => GetComponentInParent<ItemObject>();

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            // 如果玩家死亡，不执行任何操作
            if (collision.GetComponent<CharacterStats>().isDead)
                return;

            Debug.Log("Picked up item");
            // collider触碰到player时，将ItemData添加到玩家的Inventory中
            myItemObject.PickupItem();
        }
    }


}
