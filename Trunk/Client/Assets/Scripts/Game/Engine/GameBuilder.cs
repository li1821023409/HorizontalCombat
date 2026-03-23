using FileIO;
using System.Collections.Generic;
using UIFrame;
using UnityEngine;
using WNGameBase;

namespace WNEngine
{
    public class GameBuilder : UnitySingleton<GameBuilder>
    {
        #region 基础数据
        /// <summary>
        /// 是否开始游戏
        /// </summary>
        public bool IsStartGame = false;

        private const string m_PersistentSceneName = "PersistentScene";
        public string PersistentSceneName
        {
            get
            {
                return m_PersistentSceneName;
            }
        }
        /// <summary>
        /// Scene
        /// </summary>
        private string m_MapSceneName = "Scene_Farm";
        public string MapSceneName
        {
            set { m_MapSceneName = value; }
            get 
            { 
                return m_MapSceneName; 
            }
        }

        public SceneLoader SceneLoader;

        /// <summary>
        /// 搭建Scene场景，后续完善
        /// </summary>
        public virtual void BuildGameScene()
        {
            //return ReflectUtility.CreateInstance(UISceneType) as UIScene;
            BuildWNGame(SceneLoader.BuilderWNGame());
        }

        /// <summary>
        /// 创建WNGame，后续完善
        /// </summary>
        public virtual void BuildWNGame(WNGame wnGame) { DoBuildWNGame(wnGame); }

        protected virtual void DoBuildWNGame(WNGame wnGame)
        {
            wnGame.Init();
        }

        /// <summary>
        /// 创建gameInfo，后续完善
        /// </summary>
        public virtual void BuildGameInfo(GameInfo gameInfo) { DoBuildGameInfo(gameInfo); }

        protected virtual void DoBuildGameInfo(GameInfo gameInfo) 
        {
            gameInfo.Init();
        }

        /// <summary>
        /// 切换Map场景后，从场景中获取的TilemapGrid，用于从池对象中创建并移动到Map场景的对应位置
        /// </summary>
        public TilemapGrid TilemapGrid;

        protected ObjectPool ObjectPool;

        /// <summary>
        /// 将Pawn的数据存成字典，方便后续读取
        /// TODO:这里应该是异步加载的时候就要处理的，但是现在还没开始做异步加载，先在GameInfo里面处理
        /// </summary>
        protected Dictionary<string, PawnInfoData> m_PawnInfo = new Dictionary<string, PawnInfoData>();
        protected string m_PawnInfoName = "PawnInfo";

        /// <summary>
        /// 将Effect的数据存成字典，方便后续读取
        /// TODO:这里应该是异步加载的时候就要处理的，但是现在还没开始做异步加载，先在GameInfo里面处理 
        /// </summary>
        protected Dictionary<string, EffectInfoData> m_EffectInfo = new Dictionary<string, EffectInfoData>();
        protected string m_EffectInfoName = "EffectInfo";

        /// <summary>
        /// 将AssetId文件的数据存成字典，方便后续读取
        /// TODO:这里应该是异步加载的时候就要处理的，但是现在还没开始做异步加载，先在GameInfo里面处理
        /// </summary>
        protected Dictionary<string, AssetIDData> m_AssetId = new Dictionary<string, AssetIDData>();
        protected string m_AssetIdName = "AssetId";

        /// <summary>
        /// 将AssetId文件的数据存成字典，方便后续读取
        /// TODO:这里应该是异步加载的时候就要处理的，但是现在还没开始做异步加载，先在GameInfo里面处理
        /// </summary>
        protected Dictionary<string, ItemInfoData> m_ItemInfo = new Dictionary<string, ItemInfoData>();
        protected string m_ItemInfoName = "ItemInfo";

        /*下面的是PersistentScene一直存在的场景中防止池缓存的层级*/
        /// <summary>
        /// Pool根节点
        /// </summary>
        private Transform PoolRoot;
        /// <summary>
        /// 界面层级节点:Instances 这里用于创建Pawn
        /// </summary>
        public Transform InstancesPoolLevel;
        /*下面的层级节点用于创建掉落物、家具、NPC等*/
        /// <summary>
        /// 界面层级节点:BoolCanPlaceFumiture
        /// </summary>
        public Transform BoolCanPlaceFumiturePoolLevel;
        /// <summary>
        /// 界面层级节点:BoolCanFropItem
        /// </summary>
        public Transform BoolCanFropItemPoolLevel;
        /// <summary>
        /// 界面层级节点:BoolDiggable
        /// </summary>
        public Transform BoolDiggablePoolLevel;
        /// <summary>
        /// 界面层级节点:BoolPath
        /// </summary>
        public Transform BoolPathPoolLevel;
        /// <summary>
        /// 界面层级节点:BoolNpcObstacie
        /// </summary>
        public Transform BoolNpcObstaciePoolLevel;

        /// <summary>
        /// 默认PawnID为10001
        /// </summary>
        protected const string m_DefaultPawnID = "10001";
        public string DefaultPawnID
        {
            get { return m_DefaultPawnID; }
        }

        /// <summary>
        /// 默认ItemID为1001
        /// </summary>
        protected const string m_DefaultItemID = "1001";
        public string DefaultItemID
        {
            get { return m_DefaultItemID; }
        }
        #endregion

        void Awake()
        {
            SceneLoader = SceneLoader.Instance;
            ObjectPool = ObjectPool.Instance;
            // 确保不会根据场景切换而销毁
            DontDestroyOnLoad(this);
        }

        #region 加载场景
        // TODO：先跳转场景，加载对应场景的TilemapGrid，然后数据初始化获取对应参数，这里先不动态加载TilemapGrid了，直接将TilemapGrid放在场景中，加载场景完成后直接初始化获取对应TilemapGrid层
        #endregion

        #region 数据初始化
        /// <summary>
        /// 这里是进入场景需要提前加载的csv文件数据
        /// </summary>
        public void LoadAssetIDData()
        {
            // TODO:进入游戏的时候加载所有资源感觉太慢了，以后看下要不要提前加载，开始游戏的时候直接用 

            InitPawnLevel();

            m_AssetId = FileManager.Instance.ReadCSVFilesToDictionary<AssetIDData>(m_AssetIdName);
            m_PawnInfo = FileManager.Instance.ReadCSVFilesToDictionary<PawnInfoData>(m_PawnInfoName);
            m_EffectInfo = FileManager.Instance.ReadCSVFilesToDictionary<EffectInfoData>(m_EffectInfoName);
            m_ItemInfo = FileManager.Instance.ReadCSVFilesToDictionary<ItemInfoData>(m_ItemInfoName);

            foreach (string key in m_AssetId.Keys)
            {
                if (int.TryParse(key, out int assetId) && m_AssetId.ContainsKey(key))
                {
                    AssetIDData data = m_AssetId[key];
                    GameObject prefab = Resources.Load<GameObject>(data.assetPath);
                    if (prefab == null)
                    {
                        continue;
                    }
                    ObjectPool.AddPool(data.id, prefab, GetPoolLevel(int.Parse(data.type)), int.Parse(data.initialSize), int.Parse(data.maxSize));
                }
                else
                {
                    Debug.LogError($"AssetIDData with assetId '{key}' doesn't exist.");
                }
            }
            ObjectPool.InitializePools();

        }

        private void InitPawnLevel()
        {
            if (PoolRoot == null)
            {
                GameObject obj = new GameObject("PoolRoot");
                PoolRoot = obj.transform;
            }
            InstancesPoolLevel = CreatePoolLevel(GetPoolLevelName(AssetTypeEnum.Instances), (int)AssetTypeEnum.Instances);
            BoolCanPlaceFumiturePoolLevel = CreatePoolLevel(GetPoolLevelName(AssetTypeEnum.BoolCanPlaceFumiture), (int)AssetTypeEnum.BoolCanPlaceFumiture);
            BoolCanFropItemPoolLevel = CreatePoolLevel(GetPoolLevelName(AssetTypeEnum.BoolCanFropItemPool), (int)AssetTypeEnum.BoolCanFropItemPool);
            BoolDiggablePoolLevel = CreatePoolLevel(GetPoolLevelName(AssetTypeEnum.BoolDiggable), (int)AssetTypeEnum.BoolDiggable);
            BoolPathPoolLevel = CreatePoolLevel(GetPoolLevelName(AssetTypeEnum.BoolPath), (int)AssetTypeEnum.BoolPath);
            BoolNpcObstaciePoolLevel = CreatePoolLevel(GetPoolLevelName(AssetTypeEnum.BoolNpcObstacie), (int)AssetTypeEnum.BoolNpcObstacie);
        }

        /// <summary>
        /// 创建可攻击对象层级，对Pawn进行分层存放
        /// </summary>
        private Transform CreatePoolLevel(string name, int level)
        {
            GameObject obj = new GameObject(name);
            if (PoolRoot != null)
            {
                obj.transform.SetParent(PoolRoot.transform);
                return obj.transform;
            }
            else
            {
                throw new UIFrameException("pawnToot不存在");
            }
        }

        /// <summary>
        /// 获取池对象层级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetPoolLevelName(AssetTypeEnum level)
        {
            switch (level)
            {
                case AssetTypeEnum.Instances:
                    return "InstancesPoolLevel";
                case AssetTypeEnum.BoolCanPlaceFumiture:
                    return "BoolCanPlaceFumiturePoolLevel";
                case AssetTypeEnum.BoolCanFropItemPool:
                    return "BoolCanFropItemPoolLevel";
                case AssetTypeEnum.BoolDiggable:
                    return "BoolDiggablePoolLevel";
                case AssetTypeEnum.BoolPath:
                    return "BoolPathPoolLevel";
                case AssetTypeEnum.BoolNpcObstacie:
                    return "BoolNpcObstaciePoolLevel";
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取池对象层级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private Transform GetPoolLevel(int level)
        {
            if (TilemapGrid == null)
                return null;
            switch (level)
            {
                case 1:
                    return InstancesPoolLevel;
                case 2:
                    return BoolCanPlaceFumiturePoolLevel;
                case 3:
                    return BoolCanFropItemPoolLevel;
                case 4:
                    return BoolDiggablePoolLevel;
                case 5:
                    return BoolPathPoolLevel;
                case 6:
                    return BoolNpcObstaciePoolLevel;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取Map场景中的对象层级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private Transform GetTilemapGridLevel(int level)
        {
            if (TilemapGrid == null)
                return null;
            switch (level)
            {
                case 1:
                    return TilemapGrid.InstancesLevel;
                case 2:
                    return TilemapGrid.BoolCanPlaceFumitureLevel;
                case 3:
                    return TilemapGrid.BoolCanFropItemLevel;
                case 4:
                    return TilemapGrid.BoolDiggableLevel;
                case 5:
                    return TilemapGrid.BoolPathLevel;
                case 6:
                    return TilemapGrid.BoolNpcObstacieLevel;
                default:
                    return null;
            }
        }
        #endregion

        #region Pawn对象池化处理
        /// <summary>
        /// 创建Pawn对象
        /// </summary>
        public Pawn SpawnPawn(string assetId, Vector3 position, Quaternion rotation)
        {
            Pawn pawn = null;

            PawnInfo pawnInfo = ContainsPawnInfo(assetId);

            // pawnInfo 为空说明配置有问题
            if (pawnInfo != null)
            {
                GameObject pawnName = null;

                pawnName = ObjectPool.SpawnFromPool(assetId, position, rotation, GetTilemapGridLevel(pawnInfo.assetType));

                if (pawnName != null)
                {
                    // TODO：这里获取pawn组件，进行进一步处理，目前Pawn尚未完善，先处理一个即可
                    pawn = pawnName.GetComponent<Pawn>();

                    pawn.InitPawn(pawnInfo);
                }
                else
                {
                    Debug.LogError($"SpawnActor with assetId '{assetId}' doesn't exist.");
                }
            }

            return pawn;
        }

        /// <summary>
        /// 通过assetId查找对应的PawnInfo
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public PawnInfo ContainsPawnInfo(string assetId)
        {
            if (m_PawnInfo.ContainsKey(assetId))
            {
                PawnInfo pawnInfo = new PawnInfo();
                PawnInfoData pawnInfoData = m_PawnInfo[assetId];

                pawnInfo.id = pawnInfoData.id;
                pawnInfo.name = pawnInfoData.name;
                int.TryParse(pawnInfoData.type, out pawnInfo.assetType);
                float.TryParse(pawnInfoData.healthPoint, out pawnInfo.healthPoint);
                float.TryParse(pawnInfoData.attack, out pawnInfo.attack);
                float.TryParse(pawnInfoData.moveSpeed, out pawnInfo.moveSpeed);
                float.TryParse(pawnInfoData.jumpForce, out pawnInfo.jumpForce);
                float.TryParse(pawnInfoData.healthPoint, out pawnInfo.healthPoint);

                // TODO：武器这里应该也要创建表格的，但是太晚了，以后再说吧
                pawnInfo.skillID = pawnInfoData.skillID;

                return pawnInfo;
            }
            else
            {
                Debug.LogError($"Contains with assetId '{assetId}' doesn't exist.");
            }

            return null;
        }

        /// <summary>
        /// pawn死亡，返回对象池
        /// </summary>
        public void DestroyPawn(Pawn pawn)
        {
            if (pawn != null)
            {
                // 先执行pawn死亡流程
                pawn.DestroyPawn();

                // 返回对象池
                ObjectPool.ReturnToPool(pawn.m_PawnInfo.id, pawn.gameObject);
            }
        }
        #endregion

        #region Effects对象池化处理
        /// <summary>
        /// 创建Effect对象
        /// </summary>
        public void SpawnEffect(string assetId, Vector3 position, Quaternion rotation, Transform parent)
        {
            EffectInfoData effectInfo = ContainsEffectInfo(assetId);

            // pawnInfo 为空说明配置有问题

            if (effectInfo != null)
            {
                GameObject effectName = null;
                effectName = ObjectPool.SpawnFromPool(assetId, position, rotation, parent);

                if (effectName != null)
                {
                    Effect effect = effectName.GetComponent<Effect>();
                    float runTime = 0f;
                    float.TryParse(effectInfo.runTime, out runTime);
                    effect.InitEffect(effectInfo.id, effectInfo.name, runTime);
                }
                else
                {
                    Debug.LogError($"SpawnActor with assetId '{assetId}' doesn't exist.");
                }
            }
        }

        /// <summary>
        /// 通过assetId查找对应的EffectInfo
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public EffectInfoData ContainsEffectInfo(string assetId)
        {
            if (m_EffectInfo.ContainsKey(assetId))
            {
                return m_EffectInfo[assetId];
            }
            else
            {
                Debug.LogError($"Contains with assetId '{assetId}' doesn't exist.");
            }

            return null;
        }
        #endregion

        #region Item对象池化处理
        /// <summary>
        /// 给GM指令使用的Item生成
        /// 指定生成对象、位置、父物体
        /// </summary>
        public void CreateItem(string assetId, Vector3 position)
        {
            Item newItem = SpawnItem(assetId, position, Quaternion.identity, TilemapGrid.InstancesLevel)?.GetComponent<Item>();
            if (newItem != null)
            {
                newItem.Init(ContainsItemInfo(assetId));
            }
        }

        /// <summary>
        /// 创建Item对象
        /// </summary>
        public GameObject SpawnItem(string assetId, Vector3 position, Quaternion rotation, Transform parent, int initialSize = 1, int maxSize = 1)
        {
            GameObject item = null;

            if (ObjectPool.PoolContains(assetId))
            {
                item = ObjectPool.SpawnFromPool(assetId, position, rotation, parent);
            }
            else
            {
                // 没有获取说明没有添加该对象到池中，这里添加一下（也可以不添加，后面做一下处理）
                ItemInfoData data = m_ItemInfo[assetId];
                item = ObjectPool.CreateAndAddPoolObject(assetId, data.itemPath, position, rotation, parent);
                if (item == null)
                {
                    Debug.LogError($"SpawnActor with assetId '{assetId}' doesn't exist.");
                }
            }
            return item;
        }

        /// <summary>
        /// 通过assetId查找对应的ItemInfo
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public ItemInfoData ContainsItemInfo(string assetId)
        {
            ItemInfoData item = new ItemInfoData();
            if (m_ItemInfo.ContainsKey(assetId))
            {
                item = m_ItemInfo[assetId];
                return item;
            }
            else
            {
                Debug.LogError($"Contains with assetId '{assetId}' doesn't exist.");
            }
            return item;
        }

        /// <summary>
        /// 通过assetId查找对应的ItemDetails
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public ItemDetails ContainsItemDetails(string assetId)
        {
            ItemInfoData itemInfoData = ContainsItemInfo(assetId);

            if (itemInfoData != null)
            {
                ItemDetails itemDetails = new(itemInfoData)
                {
                    id = itemInfoData.id,
                    itemName = itemInfoData.itemName,
                    itemDetailedDescription = itemInfoData.itemDetailedDescription,
                    itemPath = itemInfoData.itemPath
                };
                return itemDetails;
            }

            Debug.LogError($"ItemDetails with assetId '{assetId}' doesn't exist.");
            return null;
        }

        public void DestroyItem(string itemId, GameObject item)
        {
            if (!string.IsNullOrEmpty(itemId) && item != null)
            {
                // 返回对象池
                ObjectPool.ReturnToPool(itemId, item);
            }
        }
        #endregion

        public void SetObjParent(GameObject Obj, string assetId)
        {
            // 这里设置Obj的父对象
            /*这里的想法是将所有的info都加上ID和Type，然后通过搜索Type来实现，如果没有Type的话就跳出*/
            FileData fileData = null;
            // 目前只移动Pawn，所以只判断m_PawnInfo即可，后面如果有需要再加
            fileData = m_PawnInfo[assetId];
            if (fileData != null)
            {
                int level = 0;
                if (int.TryParse(fileData.type, out level))
                {
                    Obj.transform.parent = GetTilemapGridLevel(level);
                    Debug.Log("查询层级level ： " + level);
                }
                else
                {
                    Debug.LogError($"Contains with assetId.type '{fileData.type}' doesn't exist.");
                }
            }
        }
    }
}

