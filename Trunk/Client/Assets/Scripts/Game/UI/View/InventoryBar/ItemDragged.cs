using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFrame
{
    public class ItemDragged : MonoBehaviour
    {
        /// <summary>
        /// Itemœ‘ æÕº≤„
        /// </summary>
        public Image ItemImage;

        public void SetItemDraggedData(ItemDetails itemDetails, int itemCount = 0)
        {
            ItemImage.sprite = itemDetails != null ? itemDetails.itemSprite : null;
        }
    }
}
