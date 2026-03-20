using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WNEngine;

namespace WNGameBase
{
    /// <summary>
    /// 用来控制游戏运行的基本逻辑
    /// </summary>
    public class WNGame : UnitySingleton<WNGame>
    {
        public GameUIScenes GameUIScenes;
        public CameraManager CameraManager;
        public GameBuilder GameBuilder;
        public InventoryManager InventoryManager;
        public GameInfo GameInfo;
        public InputManager InputManager;
        protected TilemapInfo TilemapInfo;

        public void Init()
        {
            BuilderGameInfo();
            BuilderGameBuilder();
            BuilderGameUIScenes();
            BuilderCameraManager();
            BuilderInventoryManager();
            BuilderInputManager();
            LoadTilemap();
            LoadResource();
            Debug.Log("[aoandouli] WNGame.Init");
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
            //GameInfo.m_LocalPlayerPawn.Tick();
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

        public void BuilderInventoryManager()
        {
            if (InventoryManager == null)
                InventoryManager = InventoryManager.Instance;
            InventoryManager.Init();
        }

        public void BuilderGameInfo()
        {
            if (GameInfo == null)
                GameInfo = GameInfo.Instance;
            GameInfo.Init();
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
            if (GameInfo.m_LocalPlayerPawn == null)
            {
                // 如果Pawn尚未创建则需要重新创建一个
                LoadPawn();
            }
            GameBuilder.SceneLoader.MoveGameObjectToScene(GameInfo.m_LocalPlayerPawn.gameObject, GameInfo.m_LocalPlayerPawnInfo.id);
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
                GameInfo.m_LocalPlayerPawns.Add(pawn);
                GameInfo.m_LocalPlayerPawn = pawn;
            }

            // TODO : 开局默认的Item是Id = 1001手，这里仅作测试用，后续移除，不存在手这个Item
            InventoryManager.AddItem(GameBuilder.DefaultItemID);
            InventoryManager.SwitchItem(GameBuilder.DefaultItemID);
        }

        public virtual void SpawnPawn(string assetID, Vector3 location, Quaternion rotate)
        {
            Pawn pawn = GameBuilder.SpawnPawn(GameBuilder.DefaultPawnID, Vector3.zero, Quaternion.identity);


            switch (pawn.m_PawnInfo.assetType)
            {
                case 0:
                    GameInfo.m_NonePawns.Add(pawn);
                    return;
                case 1:
                    GameInfo.m_LocalPlayerPawns.Add(pawn);
                    return;
                case 2:
                    GameInfo.m_FriendlyForcesPawns.Add(pawn);
                    return;
                case 3:
                    GameInfo.m_EnemyPawns.Add(pawn);
                    return;
                case 4:
                    GameInfo.m_SceneObjectPawns.Add(pawn);
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
