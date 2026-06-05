using System.Collections;
using System.Collections.Generic;
using UIFrame;
using UnityEditor;
using UnityEngine;
using WNEngine;
using WNGameTool;
using System.Linq;


namespace WNGameBase
{
    /// <summary>
    /// 武器攻击相关的都在这里管理
    /// </summary>
    public class InventoryManager : UnitySingleton<InventoryManager>
    {
        public GameInfo GameInfo;
        public GameBuilder GameBuilder;
        public PawnInfo LocalPlayerPawnInfo
        {
            get
            {
                if (GameInfo.m_LocalPlayerPawnInfo != null)
                {
                    return GameInfo.m_LocalPlayerPawnInfo;
                }
                else
                {
                    Debug.LogWarning("[aoandouli] (m_GameInfo.localPlayerPawnInfo = null");
                }
                return null;
            }
        }

        /// <summary>
        /// 当前的道具信息
        /// </summary>
        private ItemDetails m_CurrentItem = null;
        public ItemDetails CurrentItem
        {
            get { return m_CurrentItem; }
        }

        /// <summary>
        /// 切换的道具信息
        /// </summary>
        private ItemDetails m_NextItem = null;
        public ItemDetails NextItem
        {
            get { return m_NextItem; }
        }

        /// <summary>
        /// 添加到背包的道具信息
        /// </summary>
        private ItemDetails m_PreItem = null;
        public ItemDetails PreItem
        {
            get { return m_PreItem; }
        }

        /// <summary>
        /// 当前使用的道具对象
        /// </summary>
        public GameObject m_CurrentItemGameObject;


        /// <summary>
        /// 道具的父对象
        /// </summary>
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
        /// 本地玩家库存
        /// </summary>
        public InventoryDictionary LocalPawnInventory
        {
            set
            {
                if (GameInfo != null && GameInfo.m_LocalPlayerPawn != null)
                {
                    GameInfo.m_LocalPlayerPawn.inventory = value;
                }
                else
                {
                    Debug.LogError("[aoandouli] (GameInfo = null || GameInfo.m_LocalPlayerPawn = null)");
                }
            }
            get
            {
                if (GameInfo != null && GameInfo.m_LocalPlayerPawn != null)
                {
                    return GameInfo.m_LocalPlayerPawn.inventory;
                }
                else
                {
                    Debug.LogError("[aoandouli] (GameInfo = null || GameInfo.m_LocalPlayerPawn = null)");
                    return null;
                }
            }
        }
        /// <summary>
        /// 本地玩家显示栏中的道具（固定索引数组，保证顺序确定性）
        /// 道具数量统一从 LocalPawnInventory 中获取
        /// </summary>
        public ItemDetails[] LocalPawnInventoryBar
        {
            set
            {
                if (GameInfo != null && GameInfo.m_LocalPlayerPawn != null)
                {
                    GameInfo.m_LocalPlayerPawn.inventoryBar = value;
                }
                else
                {
                    Debug.LogError("[aoandouli] (GameInfo = null || GameInfo.m_LocalPlayerPawn = null)");
                }
            }
            get
            {
                if (GameInfo != null && GameInfo.m_LocalPlayerPawn != null)
                {
                    return GameInfo.m_LocalPlayerPawn.inventoryBar;
                }
                else
                {
                    Debug.LogError("[aoandouli] (GameInfo = null || GameInfo.m_LocalPlayerPawn = null)");
                    return null;
                }
            }
        }

        public void Init()
        {
            GameInfo = GameInfo.Instance;
            GameBuilder = GameBuilder.Instance;
            AddInputEvent();
        }

        /// <summary>
        /// 添加Item
        /// </summary>
        /// <param name="item">添加的item</param>
        public void AddItem(Item item, int reviseCount = 1)
        {
            if (LocalPawnInventory == null)
                return;

            // TODO：添加item的时候应该先查询后添加，这里需要注意一下
            // 先查询背包中是否包含该道具，不包含则创建一个
            m_PreItem = LocalPawnInventory.ContainsItemDetails(item.itemDetails.id);

            if (m_PreItem == null)
            {
                m_PreItem = item.itemDetails != null ? item.itemDetails : GameBuilder.ContainsItemDetails(item.itemDetails.id);
            }

            // 添加道具
            if (LocalPawnInventory.Inventory.ContainsKey(m_PreItem))
            {
                // 获取当前数量并增加新数量
                LocalPawnInventory.SetItem(m_PreItem, reviseCount);
            }
            else if (LocalPawnInventory.Inventory.Count < StaticInventoryData.INVENTORY_MAX_CARRYING_CAPACITY)
            {
                LocalPawnInventory.SetItem(m_PreItem, reviseCount);
            }
            else
            {
                Debug.LogError("[aoandouli] InventoryManager.AddItem localPawnInventory.Count >= InventoryMaxCarryingCapacity.");
            }

#if UNITY_EDITOR
            if (LocalPawnInventoryBar == null)
                return;

            // TODO：仅测试，不是最终效果
            // 这里用来测试道具栏添加功能，将道具加入显示栏固定槽位并通知UI更新
            // 查找道具是否已在显示栏中，否则分配到第一个空闲槽位
            int slotIndex = System.Array.FindIndex(LocalPawnInventoryBar, x => x != null && x.id == m_PreItem.id);
            if (slotIndex == -1)
            {
                slotIndex = System.Array.FindIndex(LocalPawnInventoryBar, x => x == null);
            }
            if (slotIndex >= 0 && slotIndex < StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY)
            {
                LocalPawnInventoryBar[slotIndex] = m_PreItem;
                // 数量统一从 LocalPawnInventory 中获取
                int count = LocalPawnInventory.GetItemCount(m_PreItem);
                Param param = new ParamBuilder().AppendObject(m_PreItem, "ItemDetails").AppendInt(slotIndex, "SlotIndex").AppendInt(count, "Count").Build();
                UIEventManager.Instance.UIEventEmit(UIEvent.NotifyUpDateInventoryBar, "", param);
            }
#endif

            m_PreItem = null;
        }

        /// <summary>
        /// 切换当前使用武器
        /// </summary>
        /// <param name="ItemId"></param>
        public void SwitchItem(Item item)
        {
            if (LocalPawnInventory == null)
                return;

            m_NextItem = LocalPawnInventory.ContainsItemDetails(item.itemDetails.id);
            if (m_NextItem == null)
            {
                // 拾取item并使用的时候可能存在背包没有的情况，这里直接添加一下
                m_NextItem = item.itemDetails != null ? item.itemDetails : GameBuilder.ContainsItemDetails(item.itemDetails.id);
                AddItem(item);
            }
            if (m_NextItem != null)
            {
                if (ItemParentTransform != null && (CurrentItem == null || item.itemDetails.id != CurrentItem.id))
                {
                    // 如果添加为当前使用道具，则需要先移除手里的（如果有），再创建该对象
                    if (CurrentItem != null && m_CurrentItemGameObject != null)
                    {
                        RemoveItem(item.itemDetails.id, m_CurrentItemGameObject);
                    }

                    m_CurrentItemGameObject = GameBuilder.SpawnItem(m_NextItem.id, Vector3.zero, Quaternion.identity, ItemParentTransform);
                    SetItemData(m_CurrentItemGameObject);
                    m_CurrentItem = m_NextItem;
                    m_NextItem = null;
                }
            }
            else
            {
                Debug.LogError("[aoandouli] InventoryManager.SwitchItem Inventory does not include : " + item.itemDetails.id);
            }
        }

        /// <summary>
        /// 设置当前Item数据
        /// </summary>
        public void SetItemData(GameObject m_CurrentItemGameObject)
        {
            GameInfo.m_LocalPlayerPawn.SetItemLayer(m_CurrentItemGameObject);
        }

        /// <summary>
        /// 移除Item
        /// </summary>
        public void RemoveItem(string itemId, GameObject itemObj)
        {
            GameBuilder.DestroyItem(itemId, itemObj);
        }

        #region 设置按键绑定
        protected virtual void AddInputEvent()
        {
            InputManager.Instance.NumberKeysEvent += SetNumberKeysParameters;
        }

        // TODO：这里暂时用不到，先放这里
        protected virtual void RemoveInputEvent()
        {
            InputManager.Instance.NumberKeysEvent += SetNumberKeysParameters;
        }

        /// <summary>
        /// 设置数字键盘参数绑定
        /// </summary>
        public void SetNumberKeysParameters(int numberKeys)
        {

        }
        #endregion
    }
}
