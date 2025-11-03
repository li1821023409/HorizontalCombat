using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WNEngine
{
    /// <summary>
    /// Pawn的基础数据
    /// TODO：之后数据多了拿出来单独创建一个脚本好了，现在先在这里
    /// </summary>
    public class PawnInfo : BaseInfo
    {
        public string id;
        public string name;
        public int assetType;
        public float healthPoint;
        public float attack;
        public float moveSpeed;
        public float jumpForce;
        public string skillID;

        private bool m_IsUseItem = true;
        /// <summary>
        /// 是否可以使用Item
        /// </summary>
        public bool IsUseItem
        {
            get
            {
                return m_IsUseItem && ItemParentTransform != null;
            }
        }


        private ItemInfoData m_CurrentItemInfo = null;
        public ItemInfoData CurrentItemInfo
        {
            set
            {
                m_CurrentItemInfo = value;
            }
            get 
            { 
                return m_CurrentItemInfo; 
            }
        }

        private Transform m_ItemParentTransform = null;
        public Transform ItemParentTransform
        {
            set
            {
                m_ItemParentTransform = value;
            }
            get
            {
                return m_ItemParentTransform;
            }
        }

    }
}