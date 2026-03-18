using UnityEngine;

// 物品详细数据，要求可序列化
[System.Serializable]
public class ItemDetails
{
    public ItemInfoData m_InfoData;

    public string id;
    public ItemType itemType = ItemType.None;
    public string itemName;
    public string itemDetailedDescription;
    public string itemPath;
    // 是初始物品吗
    public bool isStartingItem = false;
    // 可以拾取吗
    public bool canBePickedUp = false;
    // 可以丢弃吗
    public bool canBeDropped = false;
    // 可以食用吗
    public bool canBeEaten = false;
    // 可以携带吗
    public bool canBeCarried = false;

    public ItemDetails(ItemInfoData infoData)
    {
        id = infoData.id;
        itemType = (ItemType)int.Parse(infoData.type);
        itemName = infoData.itemName;
        itemDetailedDescription = infoData.itemDetailedDescription;
        itemPath = infoData.itemPath;
    }
}
