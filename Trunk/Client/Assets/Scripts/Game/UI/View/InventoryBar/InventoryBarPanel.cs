using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFrame;
using WNGameBase;

namespace UIFrame
{
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
        /// <summary>
        /// 显示栏道具（固定索引数组，与 LocalPlayerPawn.inventoryBar 槽位一一对应）
        /// </summary>
        private ItemDetails[] items = new ItemDetails[StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY];
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
        /// 通过固定槽位索引更新道具栏显示，确保遍历顺序确定性和一致性
        /// </summary>
        /// <param name="itemDetails">道具详情</param>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="count">道具数量（从 LocalPawnInventory 获取）</param>
        public void UpDateInventoryBar(ItemDetails itemDetails, int slotIndex, int count)
        {
            if (slotIndex >= 0 && slotIndex < items.Length)
            {
                items[slotIndex] = itemDetails;
                view.itemSlotList[slotIndex].SetItemSlot(itemDetails, count);
            }
        }
        #endregion
    }
}