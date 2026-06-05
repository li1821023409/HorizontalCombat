using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WNEngine;

namespace WNGameBase
{
    /// <summary>
    /// ����������Ϸ���еĻ����߼�
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
        /// �Լ����ø���Ƶ��
        /// </summary>
        protected virtual void Tick()
        {
            if (GameInfo.m_LocalPlayerPawn != null)
            {
                GameInfo.m_LocalPlayerPawn.Tick();
            }
        }

        /// <summary>
        /// ��Ϸ����UI����
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
        /// ��Ϸ��������
        /// </summary>
        public void BuilderGameBuilder()
        {
            GameBuilder = GameBuilder.Instance;
        }

        /// <summary>
        /// �������
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
        /// �����ǳ�ʼ��������Դ
        /// </summary>
        protected virtual void LoadResource()
        {
            // �ȶ�ȡAssetID��Դ�ļ�
            GameBuilder.LoadAssetIDData();
        }

        public void MovePawnToCurrentMap()
        {
            /*Ҫȷ����map��ͼ������ɲ���ȡ��TilemapGrid�ſ���*/
            if (GameInfo.m_LocalPlayerPawn == null)
            {
                // ���Pawn��δ��������Ҫ���´���һ��
                LoadPawn();
            }
            GameBuilder.SceneLoader.MoveGameObjectToScene(GameInfo.m_LocalPlayerPawn.gameObject, GameInfo.m_LocalPlayerPawnInfo.id);
        }

        /// <summary>
        /// ������ɫ
        /// </summary>
        public virtual void LoadPawn()
        {
            // ����Ĭ�Ͻ�ɫ
            LocalPlayerPawn pawn = GameBuilder.SpawnPawn(GameBuilder.DefaultPawnID, Vector3.zero, Quaternion.identity) as LocalPlayerPawn;
            if (pawn != null)
            {
                GameInfo.m_LocalPlayerPawns.Add(pawn);
                GameInfo.m_LocalPlayerPawn = pawn;
            }

            // TODO : ����Ĭ�ϵ�Item��Id = 1001�֣�������������ã������Ƴ��������������Item
            Item item = new Item();
            item.Init(GameBuilder.ContainsItemDetails(GameBuilder.DefaultItemID));
            InventoryManager.AddItem(item);
            InventoryManager.SwitchItem(item);
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

        #region Item相关逻辑
        public void ItemPickedUp(Item item, GameObject itemObj)
        {
            // 拾取物品
            InventoryManager.AddItem(item);
            GameBuilder.DestroyItem(item.itemDetails.id, itemObj);
        }
        #endregion
    }
}
