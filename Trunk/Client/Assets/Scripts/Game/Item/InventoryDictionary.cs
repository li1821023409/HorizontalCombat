using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InventoryDictionary : ScriptableObject
{
    [System.Serializable]
    public struct InventoryEntry
    {
        public ItemDetails item;
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
            if (entry.item != null)
                inventoryDictionary[entry.item] = entry.count;
        }
    }

    // 设置或更新物品
    public void SetItem(ItemDetails item, int count)
    {
        if (item == null) return;
        inventoryDictionary[item] = count;
        SyncToList();
    }

    // 获取物品数量
    public int GetItemCount(ItemDetails item)
    {
        if (item == null) return 0;
        return inventoryDictionary.TryGetValue(item, out int count) ? count : 0;
    }

    // 同步字典到序列化列表
    private void SyncToList()
    {
        inventoryEntries.Clear();
        foreach (var kvp in inventoryDictionary)
        {
            inventoryEntries.Add(new InventoryEntry { item = kvp.Key, count = kvp.Value });
        }
    }
}
