using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WNGameBase;

namespace UIFrame
{
    public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 背景底图
        /// </summary>
        public Image ItemSlotBg;
        /// <summary>
        /// Item显示图层
        /// </summary>
        public Image ItemImage;
        /// <summary>
        /// Item数量显示
        /// </summary>
        public TextMeshProUGUI ItemCount;
        /// <summary>
        /// 透明度控制组
        /// </summary>
        public CanvasGroup CanvasGroup;

        /// <summary>
        /// 当前槽位索引
        /// </summary>
        public int SlotIndex { get; set; }
        /// <summary>
        /// 当前槽位中的道具详情
        /// </summary>
        public ItemDetails CurrentItemDetails { get; set; }

        #region 拖拽事件回调
        /// <summary>
        /// 拖拽开始回调 (slotIndex)
        /// </summary>
        public System.Action<int> OnBeginDragAction;
        /// <summary>
        /// 拖拽中回调 (screenPosition)
        /// </summary>
        public System.Action<Vector2> OnDragAction;
        /// <summary>
        /// 拖拽结束回调 (slotIndex, eventData)
        /// </summary>
        public System.Action<int, PointerEventData> OnEndDragAction;
        #endregion

        private bool isDragging = false;

        public void Init()
        {
            ItemImage.sprite = null;
            ItemCount.text = null;
            CurrentItemDetails = null;
        }

        /// <summary>
        /// 物品栏显示
        /// </summary>
        public void SetItemSlot(ItemDetails itemDetails, int itemCount = 0)
        {
            if (itemDetails == null) return;

            ItemImage.sprite = itemDetails.itemSprite;
            CurrentItemDetails = itemDetails;

            if (itemCount > 0)
            {
                ItemCount.text = itemCount.ToString();
            }
        }

        /// <summary>
        /// 设置槽位可见性（拖拽时隐藏源槽位）
        /// </summary>
        public void SetSlotVisibility(bool visible)
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = visible ? 1f : 0f;
                CanvasGroup.blocksRaycasts = !isDragging;
            }
        }

        /// <summary>
        /// 清空槽位
        /// </summary>
        public void ClearSlot()
        {
            CurrentItemDetails = null;
            ItemImage.sprite = null;
            ItemCount.text = null;
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = 1f;
            }
        }

        #region 拖拽接口实现
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (CurrentItemDetails == null) return;

            isDragging = true;
            SetSlotVisibility(false);
            OnBeginDragAction?.Invoke(SlotIndex);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging || CurrentItemDetails == null) return;
            OnDragAction?.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            isDragging = false;
            OnEndDragAction?.Invoke(SlotIndex, eventData);
        }
        #endregion
    }
}
