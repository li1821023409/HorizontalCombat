using System.Collections;
using System.Collections.Generic;
using UIFrame;
using UnityEngine;
using WNEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

namespace WNGameBase
{
    public enum ItemType
    {
        None = 0,
        Weapon = 1,
        FarmTools = 2,      // 农具
        Seed = 3,           // 种子
        Food = 4,           // 食物
        Special = 5,        // 特殊类型道具
    }

    /// <summary>
    /// 武器攻击相关的都在这里管理
    /// </summary>
    public class ItemManager : UnitySingleton<ItemManager>
    {
        public GameInfo m_GameInfo;
        public GameBuilder m_GameBuilder;

        public PawnInfo m_LocalPlayerPawnInfo;
        public PawnInfo LocalPlayerPawnInfo
        {
            get
            {
                if (m_GameInfo.m_LocalPlayerPawnInfo != null)
                {
                    return m_GameInfo.m_LocalPlayerPawnInfo;
                }
                else
                {
                    Debug.LogError("[aoandouli] (m_GameInfo.localPlayerPawnInfo = null");
                }
                return null;
            }
        }

        /// <summary>
        /// 当前使用的道具信息
        /// </summary>
        private ItemInfoData m_CurrentItem;
        public ItemInfoData CurrentItem
        {
            set
            {
                if (LocalPlayerPawnInfo != null)
                {
                    LocalPlayerPawnInfo.CurrentItemInfo = value;
                }
            }
            get
            {
                if (LocalPlayerPawnInfo != null)
                {
                    return LocalPlayerPawnInfo.CurrentItemInfo;
                }
                return null;
            }
        }

        /// <summary>
        /// 当前使用的道具对象
        /// </summary>
        public GameObject m_CurrentItemGameObject;

        /// <summary>
        /// 道具的父对象
        /// </summary>
        private Transform m_ItemParentTransform = null;
        public Transform ItemParentTransform
        {
            get
            {
                if (LocalPlayerPawnInfo != null)
                {
                    return LocalPlayerPawnInfo.ItemParentTransform;
                }
                return null;
            }
        }

        /// <summary>
        /// 这里存储当前背包中所有的Item
        /// </summary>
        public List<ItemInfoData> Items = new List<ItemInfoData>();

        public void Init()
        {
            m_GameInfo = GameInfo.Instance;
            m_GameBuilder = m_GameInfo.m_GameBuilder;

            AddListener();
        }

        public void AddListener()
        {
            InputManager.Instance.NumberKeysEvent += Test;
        }

        public void RemoveListener()
        {
            InputManager.Instance.NumberKeysEvent -= Test;
        }

        /// <summary>
        /// 添加Item
        /// </summary>
        /// <param name="item">添加的item</param>
        /// <param name="isCurrentItem">是否设置为当前使用的武器</param>
        public void AddItem(ItemInfoData itemInfo)
        {
            if (!LocalPlayerPawnInfo.IsUseItem)
            {
                Debug.LogError("[aoandouli] Pawn cannot use item.");
                return;
            }

            if (!Items.Contains(itemInfo))
            {
                Items.Add(itemInfo);
            }
        }

        public void SwitchItem(string ItemId)
        {
            if (ItemParentTransform != null && (CurrentItem == null || ItemId != CurrentItem.itemId))
            {
                // 如果添加为当前使用道具，则需要先移除手里的，再创建该对象
                if (CurrentItem != null && m_CurrentItemGameObject != null)
                {
                    RemoveItem(CurrentItem, m_CurrentItemGameObject);
                }

                ItemInfoData itemInfo = m_GameBuilder.ContainsItemInfo(ItemId);

                AddItem(itemInfo);

                CurrentItem = itemInfo;
                m_CurrentItemGameObject = m_GameBuilder.SpawnItem(itemInfo.itemId, Vector3.zero, Quaternion.identity, ItemParentTransform);
                SetItemData(m_CurrentItemGameObject);
            }
        }

        /// <summary>
        /// 设置当前Item数据
        /// </summary>
        public void SetItemData(GameObject m_CurrentItemGameObject)
        {
            m_GameInfo.m_LocalPlayerPawn.SetItemLayer(m_CurrentItemGameObject);
        }

        /// <summary>
        /// 移除Item
        /// </summary>
        public void RemoveItem(ItemInfoData itemInfo, GameObject itemObj)
        {
            m_GameBuilder.DestroyItem(itemInfo.itemId, itemObj);
        }

        /// <summary>
        /// TODO：测试用，后面删除
        /// </summary>
        public void Test(int numberKeys)
        {
            if (numberKeys == 1)
            {
                SwitchItem("1001");
            }
            else if (numberKeys == 2)
            {
                SwitchItem("1002");
            }
        }
    }
}
