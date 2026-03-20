using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InventoryDictionary : ScriptableObject
{
    public Dictionary<ItemInfoData, int> inventoryDictionary = new Dictionary<ItemInfoData, int>();
    // 这里只用于显示物品种类
    public List<ItemDetails> inventoryList = new List<ItemDetails>();
}
