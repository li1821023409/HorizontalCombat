using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using WNEngine;
using WNGameTool;

namespace WNGameBase
{
    /// <summary>
    /// 玩家控制角色的父类（本地玩家控制Pawn）
    /// </summary>
    public class LocalPlayerPawn : Pawn
    {
        //protected Camera m_PlayerCamera;

        //public Camera PlayerCamera
        //{
        //    set { m_PlayerCamera = value; }
        //    get { return m_PlayerCamera; }
        //}

        #region 基础参数
        private Vector2 m_MoveVector2 = Vector2.zero;
        public Vector2 MoveVector2
        {
            get { return m_MoveVector2; }
            set { m_MoveVector2 = value; }
        }

        /// <summary>
        /// 玩家库存
        /// </summary>
        public InventoryDictionary inventory;
        /// <summary>
        /// 玩家显示栏中的道具（固定索引数组，保证顺序确定性）
        /// </summary>
        public ItemDetails[] inventoryBar = new ItemDetails[StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY];
        #endregion

        protected override void Awake()
        {
            base.Awake();
        }

        public override void InitPawn(PawnInfo pawnInfo)
        { 
            base.InitPawn(pawnInfo);
            pawnInfo.ItemParentTransform = GetItemTransform();
            // 本地玩家控制Pawn
            GameInfo.Instance.m_LocalPlayerPawnInfo = pawnInfo;

            AddInputEvent();
            CreateInventory();
        }

        public override void DestroyPawn()
        {
            base.DestroyPawn();
            RemoveInputEvent();
        }

        public override void Tick()
        {
            base.Tick();
            Move();
        }

        #region 设置按键绑定
        protected virtual void AddInputEvent()
        {
            InputManager.Instance.MovementEvent += SetMoveParameters;
        }

        protected virtual void RemoveInputEvent()
        {
            InputManager.Instance.MovementEvent += SetMoveParameters;
        }

        // 设置移动参数
        protected virtual void SetMoveParameters(Vector2 inputVector)
        {
            MoveVector2 = MoveSpeed * inputVector;
        }
        #endregion

        public virtual void Move()
        {
            m_Rigidbody2D.velocity = MoveVector2;
        }

        /// <summary>
        /// 获取当前Pawn持有Item的父对象
        /// </summary>
        /// <returns></returns>
        protected override Transform GetItemTransform()
        {
            Transform itemParent = transform.Find("ItemParent");
            return itemParent;
        }

        #region 触发器相关逻辑
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Item item = collision.gameObject.GetComponent<Item>();
            if (item != null) 
            {
                Debug.Log("[aondouli] LocalPlayerPawn.OnTriggerEnter2D Item.id : " + item.itemDetails.id + " Item.Name : " + item.itemDetails.itemName);
                // 拾取物品
                WNGame.ItemPickedUp(item, collision.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Item item = collision.gameObject.GetComponent<Item>();
            if (item != null)
            {
                Debug.Log("[aondouli] LocalPlayerPawn.OnTriggerExit2D Item.id : " + item.itemDetails.id + " Item.Name : " + item.itemDetails.itemName);
            }
        }
        #endregion

        #region 
        public void CreateInventory()
        {
            // 先判断是否已经有了LocalPawnInventory，没有就创建一个
            inventory = AssetDatabase.LoadAssetAtPath<InventoryDictionary>(StaticInventoryData.INVENTORY_DICTIONARY_PATH + StaticInventoryData.LOCAL_PAWN_INVENTORY);
            if (inventory == null)
            {
                Debug.LogWarning("[aoandouli]" + StaticInventoryData.LOCAL_PAWN_INVENTORY + ".asset 文件不存在.");
                inventory = CustomMenu.CreateInventoryDictionary(StaticInventoryData.LOCAL_PAWN_INVENTORY);
            }

            // 初始化显示栏为固定大小数组，保证遍历顺序确定性
            inventoryBar = new ItemDetails[StaticInventoryData.INVENTORY_MAX_DISPLAY_CAPACITY];
        }
        #endregion
    }
}
