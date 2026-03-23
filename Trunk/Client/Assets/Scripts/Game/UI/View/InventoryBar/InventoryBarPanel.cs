using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFrame;
using WNGameBase;
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
    private Item[] items = new Item[StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY];
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
    public void UpDateInventoryBar(Item item, int count)
    {
        // 这里自动填充，不是0就直接填充进去
        for (int i = 0; i < StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY; i++)
        {
            if (items[i] == null || items[i].ItemId == 0)
            {
                items[i] = item;
                view.itemSlotList[i].SetItemSlot(item, count);
                break;
            }
        }
    }
    #endregion
}
