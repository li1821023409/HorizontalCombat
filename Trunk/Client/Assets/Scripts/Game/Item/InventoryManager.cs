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
        private ItemInfoData m_CurrentItem = null;

        /// <summary>
        /// 切换的道具信息
        /// </summary>
        private ItemInfoData m_NextItem = null;

        /// <summary>
        /// 添加到背包的道具信息
        /// </summary>
        private ItemInfoData m_PreItem = null;

        /// <summary>
        /// 当前使用的道具信息
        /// </summary>
        public ItemInfoData CurrentItem
        {
            get { return m_CurrentItem; }
        }

        public ItemInfoData NextItem
        {
            get { return m_NextItem; }
        }

        public ItemInfoData PreItem
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

        public InventoryDictionary localPawnInventory;

        /// <summary>
        /// 这里存储当前背包中所有的Item
        /// 存储ItemDetails和数量
        /// </summary>
        public Dictionary<ItemInfoData, int> LocalPawnInventoryDictionary
        {
            get 
            {
                if (localPawnInventory == null)
                {
                    CreateInventory();
                }
                return localPawnInventory.inventoryDictionary;
            }
        }

        /// <summary>
        /// 这个仅用来编辑器调试显示
        /// </summary>
        public List<ItemDetails> LocalPawnInventoryList
        {
            get
            {
                if (localPawnInventory == null)
                {
                    CreateInventory();
                }
                return localPawnInventory.inventoryList;
            }
            set
            {
                if (localPawnInventory == null)
                {
                    CreateInventory();
                }
                localPawnInventory.inventoryList = value;
            }
        }

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

#if UNITY_EDITOR
            LocalPawnInventoryDictionary.Clear();
            LocalPawnInventoryList.Clear();
#endif
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
        public void AddItem(string ItemId, int count = 1)
        {
            Debug.LogError("[aoandouli] InventoryManager.AddItem ItemId : " + ItemId);
            m_PreItem = GameBuilder.ContainsItemInfo(ItemId);
            if (LocalPawnInventoryDictionary.ContainsKey(m_PreItem))
            {
                LocalPawnInventoryDictionary[m_PreItem] += count;
            }
            else if (LocalPawnInventoryDictionary.Count < StaticInventoryData.INVENTORY_MAX_CARRYING_CAPACITY)
            {
                LocalPawnInventoryDictionary.Add(m_PreItem, count);
            }
            else
            {
                Debug.LogError("[aoandouli] InventoryManager.AddItem localPawnInventory.Count >= InventoryMaxCarryingCapacity.");
            }

#if UNITY_EDITOR
            // 如果LocalPawnInventoryList中没有相同id的元素，则添加进去
            if (!LocalPawnInventoryList.Any(x => x.id == ItemId))
            {
                ItemDetails itemDetails = new(m_PreItem);
                LocalPawnInventoryList.Add(itemDetails);

                LocalPawnInventoryList = LocalPawnInventoryList.OrderBy(x => x.id).ToList();
            }
#endif

            m_PreItem = null;
        }

        /// <summary>
        /// 切换当前使用武器
        /// </summary>
        /// <param name="ItemId"></param>
        public void SwitchItem(string ItemId)
        {
            m_NextItem = GameBuilder.ContainsItemInfo(ItemId);
            if (m_NextItem != null)
            {
                if (ItemParentTransform != null && (CurrentItem == null || ItemId != CurrentItem.id))
                {
                    // 如果添加为当前使用道具，则需要先移除手里的（如果有），再创建该对象
                    if (CurrentItem != null && m_CurrentItemGameObject != null)
                    {
                        RemoveItem(CurrentItem, m_CurrentItemGameObject);
                    }

                    // 拾取item并使用的时候可能存在背包没有的情况，这里直接添加一下
                    if (!LocalPawnInventoryDictionary.ContainsKey(m_NextItem))
                    {
                        AddItem(ItemId);
                    }

                    m_CurrentItemGameObject = GameBuilder.SpawnItem(m_NextItem.id, Vector3.zero, Quaternion.identity, ItemParentTransform);
                    SetItemData(m_CurrentItemGameObject);
                    m_CurrentItem = m_NextItem;
                    m_NextItem = null;
                }
            }
            else
            {
                Debug.LogError("[aoandouli] InventoryManager.SwitchItem Inventory does not include : " + ItemId);
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
        public void RemoveItem(ItemInfoData itemInfo, GameObject itemObj)
        {
            GameBuilder.DestroyItem(itemInfo.id, itemObj);
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
