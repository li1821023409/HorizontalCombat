using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WNEngine;

namespace WNGameBase
{
    /// <summary>
    /// Pawn运动状态
    /// 用于动画状态基的最上层控制
    /// </summary>
    public enum MotionState
    {
        Idle = 0,           // 空闲
        Move = 1,           // 移动
        Jump = 2,           // 跳跃
        Flicker = 3,        // 闪烁（快速移动位置）
    }

    /// <summary>
    /// Pawn攻击状态与运动状态分离
    /// 用于控制动画状态基的下层
    /// </summary>
    public enum AttackState
    { 
        Idle = 0,           // 空闲
        Attack = 1,         // 攻击
        Skill = 2,          // 技能
       FlickerAttack = 3,  // 闪烁攻击（可能因为武器或某种原因在闪烁时造成伤害，这里分离出来）
    }

    public enum SortingLayerIndex
    {
        Default = 0,
        LocalPlayer = 1,
        Pawn = 2,
    }

    /// <summary>
    /// pawn脚本，所有兵卒脚本的根脚本
    /// </summary>
    public class Pawn : AttackableTarget
    {
        public PawnInfo m_PawnInfo;

        #region Pawn移动参数
        [Header("Pawn移动参数")]
        public float m_MoveSpeedMultiplier = 2;
        // 速度速率持续时间
        public float m_MoveSpeedMultiplierTime = 0;
        /// <summary>
        /// 玩家基础移动速度不能修改，修改速度只能修改玩家移动速率
        /// </summary>
        public float MoveSpeed
        {
            set { m_MoveSpeedMultiplier = value; }
            get { return m_PawnInfo.moveSpeed * m_MoveSpeedMultiplier; }
        }

        public float m_JumpSpeed = 1;
        public float m_JumpSpeedMultiplier = 1;
        /// <summary>
        /// 玩家基础跳跃速度不能修改，修改速度只能修改玩家跳跃速率
        /// </summary>
        public float JumpSpeed
        {
            set { m_JumpSpeedMultiplier = value; }
            get { return m_PawnInfo.moveSpeed * m_JumpSpeedMultiplier; }
        }
        #endregion
        /// <summary>
        /// Pawn2D碰撞器
        /// </summary>
        protected CapsuleCollider2D m_CapsuleCollider;

        /// <summary>
        /// 控制的力
        /// </summary>
        protected Rigidbody2D m_Rigidbody2D;

        //这两个状态2D不一定能用上，先放这里
        protected MotionState m_MotionState = MotionState.Idle;
        protected AttackState m_AttackState = AttackState.Idle;

        protected virtual void Awake()
        {
            m_CapsuleCollider = GetComponent<CapsuleCollider2D>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        protected virtual void start()
        {
        }

        protected virtual void LateUpdate()
        {

        }

        protected virtual void Update()
        {
        }

        protected virtual void FixedUpdate()
        {

        }

        /// <summary>
        /// 自己设置更新频率
        /// </summary>
        public virtual void Tick()
        {
            Move();
            if (Time.deltaTime % 20 == 0)
            {

            }
        }

        /// <summary>
        /// 初始化Pawn对象
        /// </summary>
        public virtual void InitPawn(PawnInfo pawnInfo)
        {
            m_PawnInfo = pawnInfo;
            SetPawnLayer();
            GetItemTransform();
        }

        protected virtual void SetPawnLayer()
        {
            if (m_Renderer == null)
            {
                m_Renderer = this.gameObject.GetComponent<SpriteRenderer>();
            }

            // 层级主要设计两个方面，一个是Obj的图层，另一个是SpriteRenderer的渲染图层
            m_Renderer.sortingLayerName = SetRenderingLayerMask(m_PawnInfo.assetType);
            m_Renderer.sortingOrder = SetRenderingLayerMaskIndex(m_PawnInfo.assetType);
            this.gameObject.ChangeLayer(SetLayer(m_PawnInfo.assetType));
        }

        public virtual void SetItemLayer(GameObject item)
        {
            SpriteRenderer itemSpriteRenderer = item.GetComponent<SpriteRenderer>();

            if (itemSpriteRenderer != null)
            {
                itemSpriteRenderer.sortingLayerName = SetRenderingLayerMask(m_PawnInfo.assetType);
                itemSpriteRenderer.sortingOrder = SetRenderingLayerMaskIndex(m_PawnInfo.assetType);
                item.ChangeLayer(SetLayer(m_PawnInfo.assetType));
            }
        }

        /// <summary>
        /// 设置SpriteRenderer渲染层级
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public virtual string SetRenderingLayerMask(int assetType)
        {
            // 根据资产类型区分层级，目前Pawn的层级只区分本地玩家和其他Pawn（Npc等，后面如果支持联机的话3P也是Pawn），其他的都是0
            switch (assetType)
            {
                case 1:
                    return "LocalPlayer";
                case 2:
                    return "Pawn";
                default:
                    return "Default";
            }
        }

        /// <summary>
        /// 设置SpriteRenderer渲染层级
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public virtual int SetRenderingLayerMaskIndex(int assetType)
        {
            // 根据资产类型区分层级，目前Pawn的层级只区分本地玩家和其他Pawn（Npc等，后面如果支持联机的话3P也是Pawn），其他的都是0
            // 注意本地玩家层级应该最高，所以3
            switch (assetType)
            {
                case 1:
                    return 3;
                case 2:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 设置Pawn的Layer层级
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public virtual string SetLayer(int assetType)
        {
            // 根据资产类型区分层级，目前Pawn的层级只区分本地玩家和其他Pawn（Npc等，后面如果支持联机的话3P也是Pawn），其他的都是0
            // 注意本地玩家层级应该最高，所以3
            switch (assetType)
            {
                case 1:
                    return "LocalPlayer";
                case 2:
                    return "Pawn";
                default:
                    return "Default";
            }
        }

        /// <summary>
        /// 这里获取Item道具创建的位置节点
        /// </summary>
        protected virtual Transform GetItemTransform()
        {
            return null;
        }

        /// <summary>
        /// 销毁Pawn对象
        /// 不是真的销毁，回到对象池中
        /// </summary>
        public virtual void DestroyPawn()
        {

        }

        /// <summary>
        /// Pawn移动相关处理
        /// </summary>
        public virtual void Move()
        {
            // 移动先这样简单处理
            m_Rigidbody2D.velocity = MoveSpeed * InputManager.Instance.MoveVector2;
        }
    }
}
