using System.Collections;
using System.Collections.Generic;
using UIFrame;
using UnityEditor;
using UnityEngine;
using WNEngine;
using WNGameTool;
using System.Linq;
using static InventoryDictionary;

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

        /// <summary>
        /// 切换的道具信息
        /// </summary>
        private ItemDetails m_NextItem = null;

        /// <summary>
        /// 添加到背包的道具信息
        /// </summary>
        private ItemDetails m_PreItem = null;

        /// <summary>
        /// 当前使用的道具信息
        /// </summary>
        public ItemDetails CurrentItem
        {
            get { return m_CurrentItem; }
        }

        public ItemDetails NextItem
        {
            get { return m_NextItem; }
        }

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
        public InventoryDictionary localPawnInventory;
        /// <summary>
        /// 本地玩家显示栏中的道具
        /// </summary>
        public InventoryDictionary localPawnInventoryBar;

        public void Init()
        {
            CreateInventory();
            GameInfo = GameInfo.Instance;
            GameBuilder = GameBuilder.Instance;
            AddListener();
        }

        public void CreateInventory()
        {
            // 先判断是否已经有了LocalPawnInventory，没有就创建一个
            localPawnInventory = AssetDatabase.LoadAssetAtPath<InventoryDictionary>(StaticInventoryData.INVENTORY_DICTIONARY_PATH + StaticInventoryData.LOCAL_PAWN_INVENTORY);
            if (localPawnInventory == null)
            {
                Debug.LogWarning("[aoandouli]" + StaticInventoryData.LOCAL_PAWN_INVENTORY + ".asset 文件不存在.");
                localPawnInventory = CustomMenu.CreateInventoryDictionary(StaticInventoryData.LOCAL_PAWN_INVENTORY);
            }

            // 先判断是否已经有了LocalPawnInventoryBar，没有就创建一个
            localPawnInventoryBar = AssetDatabase.LoadAssetAtPath<InventoryDictionary>(StaticInventoryData.INVENTORY_DICTIONARY_PATH + StaticInventoryData.LOCAL_PAWN_INVENTORY_BAR);
            if (localPawnInventoryBar == null)
            {
                Debug.LogWarning("[aoandouli]" + StaticInventoryData.LOCAL_PAWN_INVENTORY_BAR + ".asset 文件不存在.");
                localPawnInventoryBar = CustomMenu.CreateInventoryDictionary(StaticInventoryData.LOCAL_PAWN_INVENTORY_BAR);
            }
        }

        public void AddListener()
        {
            //InputManager.Instance.NumberKeysEvent += Test;
        }

        public void RemoveListener()
        {
            //InputManager.Instance.NumberKeysEvent -= Test;
        }

        /// <summary>
        /// 添加Item
        /// </summary>
        /// <param name="item">添加的item</param>
        public void AddItem(Item item, int reviseCount = 1)
        {
            // TODO：添加item的时候应该先查询后添加，这里需要注意一下
            // 先查询背包中是否包含该道具，不包含则创建一个
            m_PreItem = localPawnInventory.ContainsItemDetails(item.itemDetails.id);

            if (m_PreItem == null)
            {
                m_PreItem = item.itemDetails != null ? item.itemDetails : GameBuilder.ContainsItemDetails(item.itemDetails.id);
            }

            // 添加道具
            if (localPawnInventory.Inventory.ContainsKey(m_PreItem))
            {
                // 获取当前数量并增加新数量
                localPawnInventory.SetItem(m_PreItem, reviseCount);
            }
            else if (localPawnInventory.Inventory.Count < StaticInventoryData.INVENTORY_MAX_CARRYING_CAPACITY)
            {
                localPawnInventory.SetItem(m_PreItem, reviseCount);
            }
            else
            {
                Debug.LogError("[aoandouli] InventoryManager.AddItem localPawnInventory.Count >= InventoryMaxCarryingCapacity.");
            }

#if UNITY_EDITOR
            // 这里用来测试道具栏添加功能，道具栏数量没有达到最大，添加到道具栏并通知UI更新
            // TODO：仅测试，不是最终效果
            if (localPawnInventoryBar.GetItemCount(m_PreItem) <= StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY)
            {
                localPawnInventoryBar.SetItem(m_PreItem, reviseCount);
                Param param = new ParamBuilder().AppendObject(m_PreItem, "ItemDetails").AppendInt(localPawnInventoryBar.GetItemCount(m_PreItem), "Count").Build();
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
            m_NextItem = localPawnInventory.ContainsItemDetails(item.itemDetails.id);
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

        ///// <summary>
        ///// TODO：测试用，后面删除
        ///// </summary>
        //public void Test(int numberKeys)
        //{
        //    if (numberKeys == 1)
        //    {
        //        SwitchItem("1001");
        //    }
        //    else if (numberKeys == 2)
        //    {
        //        SwitchItem("1002");
        //    }

        //    if (numberKeys == 6)
        //    {
        //        GameBuilder.MapSceneName = "Scene_Farm";
        //        GameBuilder.SceneLoader.BuildMapScene(GameBuilder.MapSceneName);
        //    }

        //    if (numberKeys == 7)
        //    {
        //        GameBuilder.MapSceneName = "Scene_Farm 1";
        //        GameBuilder.SceneLoader.BuildMapScene(GameBuilder.MapSceneName);
        //    }
        //}
    }
}
