using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UIFrame
{
    public class ItemDragged : MonoBehaviour
    {
        /// <summary>
        /// Item��ʾͼ��
        /// </summary>
        public Image ItemImage;
        /// <summary>
        /// 透明度控制组
        /// </summary>
        public CanvasGroup CanvasGroup;

        void Awake()
        {
            if (CanvasGroup == null)
                CanvasGroup = GetComponent<CanvasGroup>();
            SetHidden(true);
        }
 
        public void SetItemDraggedData(ItemDetails itemDetails, int itemCount = 0)
        {
            ItemImage.sprite = itemDetails != null ? itemDetails.itemSprite : null;
        } 

        public void SetHidden(bool isHidden = true)
        {
            if (CanvasGroup == null)
            {
                this.gameObject.SetActive(!isHidden);
            }
            else
            {
                CanvasGroup.alpha = isHidden ? 0.0f : 1.0f;
            }
        }
    }
}
