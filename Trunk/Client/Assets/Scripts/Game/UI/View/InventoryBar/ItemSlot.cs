using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WNGameBase;

namespace UIFrame
{
    public class ItemSlot : MonoBehaviour
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

        public void Init()
        {
            ItemImage.sprite = null;
            ItemCount.text = null;
        }

        public void SetItemSlot(ItemDetails itemDetails, int itemCount = 0)
        {
            ItemImage.sprite = itemDetails != null ? itemDetails.itemSprite : null;

            if (itemCount > 0)
            {
                ItemCount.text = itemCount.ToString();
            }
        }
    }
}
