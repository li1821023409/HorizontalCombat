using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFrame;
using WNGameBase;
using System.Linq;

public class InventoryBarPanel : Panel
{
    #region 基础方法
    public override ViewCanvasLevel canvasLevel => ViewCanvasLevel.MAIN;
    public override ViewCoverType coverType => ViewCoverType.IN_CONTAINER;
    public override void OnCreate() => View = new InventoryBarView();
    public override string resPath => "Prefabs/UI/InventoryBar/InventoryBar";
    #endregion

    #region 基础数据
    private InventoryBarView view;
    //private ItemDetails[] items = new ItemDetails[StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY];
    private List<ItemDetails> itemDetailsList = new List<ItemDetails>();
    #endregion

    #region 生命周期
    public override void OnShow()
    {

    }

    public override void OnHide()
    {

    }

    public override void OnClose()
    {
    }

    public override void OnBeforeShow()
    {

    }

    public override void OnViewLoaded()
    {
        view = panleView as InventoryBarView;
    }
    #endregion

    #region 逻辑处理
    /// <summary>
    /// 更新物品栏显示
    /// </summary>
    public void UpDateInventoryBar(ItemDetails itemDetails, int count)
    {
        // TODO：仅测试，拾取后会自动添加到物品栏中
        int index = itemDetailsList.FindIndex(x => x.id == itemDetails.id);

        if (index == -1 && itemDetailsList.Count <= StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY)
        {
            itemDetailsList.Add(itemDetails);
            view.itemSlotList[itemDetailsList.Count - 1].SetItemSlot(itemDetails, count);
        }
        else if (index <= StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY)
        {
            view.itemSlotList[index].SetItemSlot(itemDetails, count);
        }
    }
    #endregion
}
