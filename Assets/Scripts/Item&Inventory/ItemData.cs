using UnityEngine;

public enum ItemType
{
    Material,
    Equipment,
}



[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType; // 物品类型

    public string itemName; // 物品名称
    public Sprite icon; // 物品图标

}
