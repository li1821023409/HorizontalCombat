using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有item基类
/// </summary>
namespace WNGameBase
{
    // 必须包含2D碰撞组件
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
            if (itemdetails == null)
            {
                this.itemDetails = itemdetails;
                int.TryParse(itemdetails.id, out itemId);
            }
        }

        #region 可拖拽晃动相关逻辑
        // 暂时用协程实现（后面看下是否用动画）
        Coroutine dragCoroutine;

        public void CanBeDrag(Vector3 velocity)
        {
            if (!itemDetails.canBeDrag)
            {
                return;
            }

            // 暂时打算收到速度影响，后面再看
        }

        private IEnumerable DragCoroutine()
        {
            yield return null;
        }
        #endregion
    }
}
