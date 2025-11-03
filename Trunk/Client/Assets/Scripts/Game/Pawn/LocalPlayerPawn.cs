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
        }

        public override void Tick()
        {
            base.Tick();
        }

        public virtual void Move(Vector2 inputVector)
        {
            m_MoveVector2 = MoveSpeed * inputVector;
            m_Rigidbody2D.velocity = m_MoveVector2;
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
    }
}
