using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UIFrame;
using WNGameBase;
using WNEngine;

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

        #region 拖拽状态
        private int currentDraggingSlotIndex = -1;
        private RectTransform draggedItemRectTransform;
        private Transform draggedItemOriginalParent;
        private Canvas rootCanvas;

        // 保存拖拽对象原始 RectTransform 状态，用于拖拽结束时恢复
        private Vector2 draggedItemOriginalAnchorMin;
        private Vector2 draggedItemOriginalAnchorMax;
        private Vector2 draggedItemOriginalPivot;
        private Vector2 draggedItemOriginalSizeDelta;
        #endregion
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
            rootCanvas = view.viewGameObject?.GetComponentInParent<Canvas>();
            SetupSlotEvents();
        }
        #endregion

        #region 槽位事件绑定
        /// <summary>
        /// 为所有槽位绑定拖拽事件回调
        /// </summary>
        private void SetupSlotEvents()
        {
            for (int i = 0; i < view.itemSlotList.Count; i++)
            {
                int index = i;
                ItemSlot slot = view.itemSlotList[index];
                slot.SlotIndex = index;
                slot.OnBeginDragAction = OnSlotBeginDrag;
                slot.OnDragAction = OnSlotDrag;
                slot.OnEndDragAction = OnSlotEndDrag;
            }
        }
        #endregion

        #region 拖拽回调
        /// <summary>
        /// 拖拽开始：隐藏源槽位，显示拖拽跟随对象
        /// </summary>
        private void OnSlotBeginDrag(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= items.Length || items[slotIndex] == null)
                return;

            currentDraggingSlotIndex = slotIndex;

            // 设置拖拽跟随对象的图片
            ItemDragged dragged = view.ItemDragged;
            if (dragged == null) return;

            dragged.SetItemDraggedData(items[slotIndex]);

            // 将拖拽对象挂到 Canvas 根节点，避免被道具栏裁剪
            draggedItemRectTransform = dragged.GetComponent<RectTransform>();
            draggedItemOriginalParent = draggedItemRectTransform.parent;

            // 保存原始 RectTransform 状态，以便拖拽结束后恢复
            draggedItemOriginalAnchorMin = draggedItemRectTransform.anchorMin;
            draggedItemOriginalAnchorMax = draggedItemRectTransform.anchorMax;
            draggedItemOriginalPivot = draggedItemRectTransform.pivot;
            draggedItemOriginalSizeDelta = draggedItemRectTransform.sizeDelta;

            if (rootCanvas != null)
            {
                // 保存世界位置后重新挂载到 Canvas 根节点
                Vector3 worldPos = draggedItemRectTransform.position;
                draggedItemRectTransform.SetParent(rootCanvas.transform, false);

                // 重置 anchors 和 pivot 为中心，确保 anchoredPosition 与
                // ScreenPointToLocalPointInRectangle 返回的 Canvas 坐标一致
                draggedItemRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                draggedItemRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                draggedItemRectTransform.pivot = new Vector2(0.5f, 0.5f);

                // 恢复到挂载前的世界位置
                draggedItemRectTransform.position = worldPos;

                draggedItemRectTransform.SetAsLastSibling();
            }

            dragged.SetHidden(false);
        }

        /// <summary>
        /// 拖拽中：更新拖拽跟随对象位置
        /// </summary>
        private void OnSlotDrag(Vector2 screenPosition)
        {
            if (currentDraggingSlotIndex < 0 || draggedItemRectTransform == null)
                return;

            // 将屏幕坐标转换为 Canvas 本地坐标
            if (rootCanvas != null)
            {
                RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPosition, rootCanvas.worldCamera, out Vector2 localPoint);
                draggedItemRectTransform.anchoredPosition = localPoint;
            }
        }

        /// <summary>
        /// 拖拽结束：判断放置位置，执行成功放置或回退
        /// </summary>
        private void OnSlotEndDrag(int slotIndex, PointerEventData eventData)
        {
            // 隐藏拖拽跟随对象
            HideDraggedVisual();

            if (currentDraggingSlotIndex < 0)
            {
                RestoreSlotVisibility(slotIndex);
                return;
            }

            currentDraggingSlotIndex = -1;

            // 如果鼠标在 UI 上，说明未放置到地图，取消拖拽
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // 拖拽取消，恢复源槽位显示
                RestoreSlotVisibility(slotIndex);
                return;
            }

            // 尝试放置道具到地图
            if (TryPlaceItemOnMap(slotIndex))
            {
                // 放置成功，槽位保持隐藏状态（InventoryManager 会移除数据并触发 UI 刷新）
                return;
            }

            // 放置失败，恢复源槽位显示
            RestoreSlotVisibility(slotIndex);
        }
        #endregion

        #region 拖拽辅助
        /// <summary>
        /// 隐藏拖拽跟随对象
        /// </summary>
        private void HideDraggedVisual()
        {
            ItemDragged dragged = view.ItemDragged;
            if (dragged == null) return;

            dragged.SetHidden(true);

            // 恢复拖拽对象到原始父节点
            if (draggedItemRectTransform != null && draggedItemOriginalParent != null)
            {
                // 先恢复原始 RectTransform 状态，再放回原父节点
                draggedItemRectTransform.anchorMin = draggedItemOriginalAnchorMin;
                draggedItemRectTransform.anchorMax = draggedItemOriginalAnchorMax;
                draggedItemRectTransform.pivot = draggedItemOriginalPivot;
                draggedItemRectTransform.sizeDelta = draggedItemOriginalSizeDelta;

                draggedItemRectTransform.SetParent(draggedItemOriginalParent, false);
                draggedItemOriginalParent = null;
            }
            draggedItemRectTransform = null;
        }

        /// <summary>
        /// 恢复指定槽位的可见性
        /// </summary>
        private void RestoreSlotVisibility(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < view.itemSlotList.Count)
            {
                view.itemSlotList[slotIndex].SetSlotVisibility(true);
            }
        }
        #endregion

        #region 地图放置逻辑
        /// <summary>
        /// 尝试将道具放置到地图上（BoolCanFropItemLevel 层级）
        /// </summary>
        /// <returns>是否成功放置</returns>
        private bool TryPlaceItemOnMap(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= items.Length)
                return false;

            ItemDetails details = items[slotIndex];
            if (details == null) return false;

            // 获取地图放置层级
            Transform dropParent = GetDropParent();
            if (dropParent == null) return false;

            // 屏幕坐标转世界坐标
            Vector3 mousePos = Input.mousePosition;
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return false;

            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;

            // 在地图上生成道具
            GameBuilder gameBuilder = GameBuilder.Instance;
            if (gameBuilder == null) return false;

            GameObject itemObj = gameBuilder.SpawnItem(
                details.id, worldPos, Quaternion.identity, dropParent);
            if (itemObj == null) return false;

            // 初始化道具的 Item 组件
            Item itemComponent = itemObj.GetComponent<Item>();
            if (itemComponent != null)
            {
                itemComponent.Init(details);
            }

            // 从背包和显示栏中移除道具
            InventoryManager.Instance.RemoveItemFromBar(slotIndex);

            return true;
        }

        /// <summary>
        /// 获取地图上用于放置丢弃道具的层级 Transform
        /// </summary>
        private Transform GetDropParent()
        {
            GameBuilder gameBuilder = GameBuilder.Instance;
            if (gameBuilder == null || gameBuilder.TilemapGrid == null)
                return null;

            return gameBuilder.TilemapGrid.BoolCanFropItemLevel;
        }
        #endregion

        #region 逻辑处理
        /// <summary>
        /// 通过固定槽位索引更新道具栏显示，确保遍历顺序确定性和一致性
        /// </summary>
        public void UpDateInventoryBar(ItemDetails itemDetails, int slotIndex, int count)
        {
            if (slotIndex >= 0 && slotIndex < items.Length)
            {
                items[slotIndex] = itemDetails;
                ItemSlot slot = view.itemSlotList[slotIndex];
                slot.SlotIndex = slotIndex;
                slot.CurrentItemDetails = itemDetails;
                slot.SetItemSlot(itemDetails, count);
            }
        }

        /// <summary>
        /// 从显示栏中移除指定槽位的道具（由 InventoryManager 触发）
        /// </summary>
        public void RemoveItemFromBar(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < items.Length)
            {
                items[slotIndex] = null;
                view.itemSlotList[slotIndex].ClearSlot();
            }
        }
        #endregion
    }
}
