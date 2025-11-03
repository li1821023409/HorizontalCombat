using System.Collections;
using System.Collections.Generic;
using UIFrame;
using UnityEngine;
using UnityEngine.InputSystem;
using WNEngine;

namespace WNGameBase
{
    /// <summary>
    /// 用来控制游戏运行的基本逻辑
    /// </summary>
    public class GameInfo : UnitySingleton<GameInfo>
    {
        private GameUIScenes m_GameUIScenes;
        private CameraManager m_CameraManager;
        public GameBuilder m_GameBuilder;
        private ItemManager m_ItemManager;
        protected TilemapInfo m_TilemapInfo;
        private InputManager m_InputManager;

        /// <summary>
        /// 注意，游戏只有这里能获取，其他地方都不行
        /// </summary>
        public TilemapInfo TilemapInfo
        {
            get { return m_TilemapInfo; }
        }

        /// <summary>
        /// NonePawns列表
        /// </summary>
        public List<Pawn> m_NonePawns = new List<Pawn>();

        /// <summary>
        /// 玩家可能存在可以切换的Pawn对象，这里创建成列表
        /// </summary>
        public List<Pawn> m_LocalPlayerPawns = new List<Pawn>();

        /// <summary>
        /// FriendlyForcesPawns列表
        /// </summary>
        public List<Pawn> m_FriendlyForcesPawns = new List<Pawn>();

        /// <summary>
        /// EnemyPawns列表
        /// </summary>
        public List<Pawn> m_EnemyPawns = new List<Pawn>();

        /// <summary>
        /// SceneObjectPawns列表
        /// </summary>
        public List<Pawn> m_SceneObjectPawns = new List<Pawn>();

        public Pawn m_LocalPlayerPawn = null;

        public PawnInfo m_LocalPlayerPawnInfo = null;

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {

        }

        public void Init()
        {
            BuilderGameBuilder();
            BuilderGameUIScenes();
            BuilderCameraManager();
            BuilderItemManager();
            BuilderInputManager();
            LoadTilemap();
            LoadResource();
            Debug.Log("[aoandouli] GameInfo.Awake");
        }

        protected virtual void Update()
        {
            Tick();
        }

        /// <summary>
        /// 自己设置更新频率
        /// </summary>
        protected virtual void Tick()
        {
            m_InputManager.Tick();
            m_LocalPlayerPawn.Tick();
        }

        /// <summary>
        /// 游戏基本UI构建
        /// </summary>
        public void BuilderGameUIScenes()
        {
            if (m_GameUIScenes == null)
                m_GameUIScenes = GameUIScenes.Instance;
            m_GameUIScenes.Init();

            UIEventManager.Instance.Emit(UIEvent.NotifyInitialPanel);
            UIEventManager.Instance.Emit(UIEvent.NotifyDialogueRootPanel);
        }

        /// <summary>
        /// 游戏基本UI构建
        /// </summary>
        public void BuilderGameBuilder()
        {
            // gameBuilder现在是空的，先创建一个
            m_GameBuilder = new GameBuilder();
        }

        /// <summary>
        /// 相机控制
        /// </summary>
        public void BuilderCameraManager()
        {
            if (m_CameraManager == null)
                m_CameraManager = CameraManager.Instance;
            m_CameraManager.Init();
        }

        /// <summary>
        /// 相机控制
        /// </summary>
        public void BuilderItemManager()
        {
            if (m_ItemManager == null)
                m_ItemManager = ItemManager.Instance;
            m_ItemManager.Init();
        }

        protected virtual void BuilderInputManager()
        {
            if (m_InputManager == null)
                m_InputManager = InputManager.Instance;
            m_InputManager.Init();
        }

        protected virtual void LoadTilemap()
        {
            m_TilemapInfo = new TilemapInfo();
            if (m_TilemapInfo != null)
            {
                m_TilemapInfo.InitTilemap();
            }
        }

        /// <summary>
        /// 这里是初始化加载资源
        /// </summary>
        protected virtual void LoadResource()
        {
            // 先读取AssetID资源文件
            m_GameBuilder.LoadAssetIDData();

            // 创建默认角色
            Pawn pawn = m_GameBuilder.SpawnPawn(m_GameBuilder.DefaultPawnID, Vector3.zero, Quaternion.identity);
            if (pawn != null)
            {
                m_LocalPlayerPawns.Add(pawn);
                m_LocalPlayerPawn = pawn;
            }

            // TODO : 开局默认的Item是Id = 1001手，这里仅作测试用，后续移除，不存在手这个Item
            m_ItemManager.SwitchItem(m_GameBuilder.DefaultItemID);
        }

        public virtual void SpawnPawn(string assetID, Vector3 location, Quaternion rotate)
        {
            Pawn pawn = m_GameBuilder.SpawnPawn(m_GameBuilder.DefaultPawnID, Vector3.zero, Quaternion.identity);


            switch (pawn.m_PawnInfo.assetType)
            {
                case 0:
                    m_NonePawns.Add(pawn);
                    return;
                case 1:
                    m_LocalPlayerPawns.Add(pawn);
                    return;
                case 2:
                    m_FriendlyForcesPawns.Add(pawn);
                    return;
                case 3:
                    m_EnemyPawns.Add(pawn);
                    return;
                case 4:
                    m_SceneObjectPawns.Add(pawn);
                    return;
            }
        }

        public virtual void DestroyPawn(Pawn pawn)
        {
            m_GameBuilder.DestroyPawn(pawn);
        }

        protected void ItemButtonInput(InputAction.CallbackContext Obj)
        {

        }

    }
}
