using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item类定义
/// </summary>
namespace WNGameBase
{
    // 需要绑定2D碰撞器
    [RequireComponent(typeof(Collider2D))] 
    public class Item : MonoBehaviour
    {
        // item详细信息
        public ItemDetails itemDetails;

        private int itemId = 0;
        public int ItemId
        {
            get
            {
                return itemId;
            }
        }

        public void Init(ItemDetails itemdetails)
        {
            if (itemdetails != null)
            {
                this.itemDetails = itemdetails;
                int.TryParse(itemdetails.id, out itemId);
            }
        }

        public void Init(ItemInfoData itemInfoData)
        {
            if (itemInfoData != null)
            {
                this.itemDetails.SetInfoData(itemInfoData);
                int.TryParse(itemDetails.id, out itemId);
            }
        }

        #region 物品拖拽相关逻辑
        // 暂时用协程实现，后续看是否需要独立类
        Coroutine dragCoroutine;

        public void CanBeDrag(Vector3 velocity)
        {
            if (!itemDetails.canBeDrag)
            {
                return;
            }

            // 暂时未受到速度影响，后续再看
        }

        private IEnumerable DragCoroutine()
        {
            yield return null;
        }
        #endregion
    }
}
