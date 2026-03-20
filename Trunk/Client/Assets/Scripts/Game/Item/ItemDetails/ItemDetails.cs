using UnityEngine;

// 物品详细数据，要求可序列化
[System.Serializable]
public class ItemDetails
{
    public ItemInfoData m_InfoData;

    /*下面数据要放在列表中*/
    public string id;
    public ItemType itemType = ItemType.None;
    public string itemName;
    public SpriteRenderer itemSprite;
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
    // 可以拖拽吗（pawn经过时会晃动）
    public bool canBeDrag = false;
    // 可以格子重叠吗（数量大于1放在一个格子里）
    public bool canBeOverlap = false;
    //// item数量
    //public int itemCount = 1;

    public ItemDetails(ItemInfoData infoData)
    {
        id = infoData.id;
        itemType = (ItemType)int.Parse(infoData.type);
        itemName = infoData.itemName;
        itemDetailedDescription = infoData.itemDetailedDescription;
        itemPath = infoData.itemPath;
    }
}
