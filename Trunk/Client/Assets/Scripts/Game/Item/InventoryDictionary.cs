using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class InventoryDictionary : ScriptableObject
{
    [System.Serializable]
    public struct InventoryEntry
    {
        public ItemDetails itemDetails;
        public int count;
    }

    [SerializeField]
    private List<InventoryEntry> inventoryEntries = new List<InventoryEntry>();

    // 用于运行时的字典，便于查找
    private Dictionary<ItemDetails, int> inventoryDictionary = new Dictionary<ItemDetails, int>();

    public IReadOnlyDictionary<ItemDetails, int> Inventory => inventoryDictionary;

    private void OnEnable()
    {
        inventoryDictionary.Clear();
        foreach (var entry in inventoryEntries)
        {
            if (entry.itemDetails != null)
                inventoryDictionary[entry.itemDetails] = entry.count;
        }
    }

    // 设置或更新物品
    public void SetItem(ItemDetails itemDetails, int reviseCount)
    {
        if (itemDetails == null) return;
        if (!inventoryDictionary.ContainsKey(itemDetails) && reviseCount > 0)
        {
            inventoryDictionary.Add(itemDetails, reviseCount);
        }
        else if (GetItemCount(itemDetails) + reviseCount > 0)
        {
            inventoryDictionary[itemDetails] = GetItemCount(itemDetails) + reviseCount;
        }
        else
        {
            inventoryDictionary.Remove(itemDetails);
        }
        SyncToList();
    }

    // 获取物品数量
    public int GetItemCount(ItemDetails itemDetails)
    {
        if (itemDetails == null) return 0;
        return inventoryDictionary.TryGetValue(itemDetails, out int count) ? count : 0;
    }

    // 同步字典到序列化列表
    private void SyncToList()
    {
        inventoryEntries.Clear();
        foreach (var kvp in inventoryDictionary)
        {
            inventoryEntries.Add(new InventoryEntry { itemDetails = kvp.Key, count = kvp.Value });
        }
    }

    public ItemDetails ContainsItemDetails(string itemId)
    {
        // 查询inventoryEntries中key中ItemDetails.m_InfoData是否包含itemInfoData
        InventoryEntry inventoryEntry = inventoryEntries.Find(item => item.itemDetails.id == itemId);
        return inventoryEntry.itemDetails;
    }
}
