using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFrame;
using Unity.VisualScripting;

public class InventoryBarView : BaseView
{
    public Transform ItemBar => GetUI("Bar");
    private Transform ItemSlotTransform => GetUI("ItemSlot");

    public List<ItemSlot> itemSlotList = new List<ItemSlot>();

    protected override void OnAttach()
    {
        if (ItemSlotTransform == null || ItemBar == null) return;

        itemSlotList.Clear();
        CreateAndInitializeItemSlots();
    }

    private void CreateAndInitializeItemSlots()
    {
        ItemSlot initialItemSlot = ItemSlotTransform.GetComponent<ItemSlot>();
        if (initialItemSlot == null) return;

        AddItemSlotToList(initialItemSlot);

        int slotsToCreate = StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY - 1;
        for (int i = 0; i < slotsToCreate; i++)
        {
            ItemSlot newItemSlot = InstantiateItemSlot();
            if (newItemSlot != null)
            {
                AddItemSlotToList(newItemSlot);
            }
        }
    }

    private ItemSlot InstantiateItemSlot()
    {
        Transform newSlotTransform = GameObject.Instantiate(ItemSlotTransform, ItemBar);
        return newSlotTransform?.GetComponent<ItemSlot>();
    }

    private void AddItemSlotToList(ItemSlot itemSlot)
    {
        itemSlot.Init();
        itemSlotList.Add(itemSlot);
    }
}
