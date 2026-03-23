using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WNEngine;

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

        protected virtual void AddInputEvent()
        {
            InputManager.Instance.MovementEvent += SetMoveParameters;
        }

        protected virtual void RemoveInputEvent()
        {
            InputManager.Instance.MovementEvent += SetMoveParameters;
        }

        public virtual void Move()
        {
            m_Rigidbody2D.velocity = MoveVector2;
        }

        // 设置移动参数
        protected virtual void SetMoveParameters(Vector2 inputVector)
        {
            MoveVector2 = MoveSpeed * inputVector;
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
    }
}
