using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����item����
/// </summary>
namespace WNGameBase
{
    // �������2D��ײ���
    [RequireComponent(typeof(Collider2D))] 
    public class Item : MonoBehaviour
    {
        // item��ϸ��Ϣ
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

        #region ����ק�ζ�����߼�
        // ��ʱ��Э��ʵ�֣����濴���Ƿ��ö�����
        Coroutine dragCoroutine;

        public void CanBeDrag(Vector3 velocity)
        {
            if (!itemDetails.canBeDrag)
            {
                return;
            }

            // ��ʱ�����յ��ٶ�Ӱ�죬�����ٿ�
        }

        private IEnumerable DragCoroutine()
        {
            yield return null;
        }
        #endregion
    }
}
