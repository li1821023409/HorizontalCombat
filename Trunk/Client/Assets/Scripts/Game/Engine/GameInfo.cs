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
    public class GameInfo : Singleton<GameInfo>
    {
        private GameUIScenes GameUIScenes;
        private CameraManager CameraManager;
        public GameBuilder GameBuilder;
        private ItemManager ItemManager;
        protected TilemapInfo TilemapInfo;
        private InputManager InputManager;

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
            m_LocalPlayerPawn.Tick();
        }

        /// <summary>
        /// 游戏基本UI构建
        /// </summary>
        public void BuilderGameUIScenes()
        {
            if (GameUIScenes == null)
                GameUIScenes = GameUIScenes.Instance;
            GameUIScenes.Init();

            UIEventManager.Instance.UIEventEmit(UIEvent.NotifyInitialPanel);
            //UIEventManager.Instance.UIEventEmit(UIEvent.NotifyDialogueRootPanel);
        }

        /// <summary>
        /// 游戏基本构建
        /// </summary>
        public void BuilderGameBuilder()
        {
            GameBuilder = GameBuilder.Instance;
        }

        /// <summary>
        /// 相机控制
        /// </summary>
        public void BuilderCameraManager()
        {
            if (CameraManager == null)
                CameraManager = CameraManager.Instance;
            CameraManager.Init();
        }

        /// <summary>
        /// 相机控制
        /// </summary>
        public void BuilderItemManager()
        {
            if (ItemManager == null)
                ItemManager = ItemManager.Instance;
            ItemManager.Init();
        }

        protected virtual void BuilderInputManager()
        {
            if (InputManager == null)
                InputManager = InputManager.Instance;
            InputManager.Init();
        }

        protected virtual void LoadTilemap()
        {
            TilemapInfo = new TilemapInfo();
            if (TilemapInfo != null)
            {
                TilemapInfo.InitTilemap();
            }
        }

        /// <summary>
        /// 这里是初始化加载资源
        /// </summary>
        protected virtual void LoadResource()
        {
            // 先读取AssetID资源文件
            GameBuilder.LoadAssetIDData();
        }

        public void MovePawnToCurrentMap()
        {
            /*要确保再map地图加载完成并读取到TilemapGrid才可以*/
            if (m_LocalPlayerPawn == null)
            {
                // 如果Pawn尚未创建则需要重新创建一个
                LoadPawn();
            }
            else
            {
                GameBuilder.SceneLoader.MoveGameObjectToScene(m_LocalPlayerPawn.gameObject);
            }
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        public virtual void LoadPawn()
        {
            // 创建默认角色
            Pawn pawn = GameBuilder.SpawnPawn(GameBuilder.DefaultPawnID, Vector3.zero, Quaternion.identity);
            if (pawn != null)
            {
                m_LocalPlayerPawns.Add(pawn);
                m_LocalPlayerPawn = pawn;
            }

            // TODO : 开局默认的Item是Id = 1001手，这里仅作测试用，后续移除，不存在手这个Item
            ItemManager.SwitchItem(GameBuilder.DefaultItemID);
        }

        public virtual void SpawnPawn(string assetID, Vector3 location, Quaternion rotate)
        {
            Pawn pawn = GameBuilder.SpawnPawn(GameBuilder.DefaultPawnID, Vector3.zero, Quaternion.identity);


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
            GameBuilder.DestroyPawn(pawn);
        }

        protected void ItemButtonInput(InputAction.CallbackContext Obj)
        {

        }
    }
}
