using UnityEngine;
using UnityEngine.UI;

// 商品详细数据，需要序列化
[System.Serializable]
public class ItemDetails
{
    public ItemInfoData m_InfoData;

    /*以下字段需要补充完整*/
    public string id = "";
    public ItemType itemType = ItemType.None;
    public string itemName = "";
    public Sprite itemSprite = null;
    public string itemDetailedDescription = "";
    public string itemPath = "";
    // 是否初始物品
    public bool isStartingItem = false;
    // 是否可拾取
    public bool canBePickedUp = false;
    // 是否可丢弃
    public bool canBeDropped = false;
    // 是否可食用
    public bool canBeEaten = false;
    // 是否可携带
    public bool canBeCarried = false;
    // 是否可拖拽（pawn状态时拖拽）
    public bool canBeDrag = false;
    // 是否可叠加（叠加数量大于1时为可叠加）
    public bool canBeOverlap = false;
    //// item数量
    //public int itemCount = 1;

    public ItemDetails(ItemInfoData infoData)
    {
        id = infoData.id;
        itemName = infoData.itemName;
        itemDetailedDescription = infoData.itemDetailedDescription;
        itemPath = infoData.itemPath;
    }

    public void SetInfoData(ItemInfoData infoData)
    {
        m_InfoData = infoData;
        id = infoData.id;
        itemName = infoData.itemName;
        itemDetailedDescription = infoData.itemDetailedDescription;
        itemPath = infoData.itemPath;
    }

    public void SetInfoData(ItemDetails itemInfoData)
    {
        m_InfoData = itemInfoData.m_InfoData;
        id = itemInfoData.m_InfoData.id;
        itemName = itemInfoData.m_InfoData.itemName;
        itemDetailedDescription = itemInfoData.m_InfoData.itemDetailedDescription;
        itemPath = itemInfoData.m_InfoData.itemPath;
    }
}
