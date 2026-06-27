using FileIO;
using System;
using System.Collections.Generic;
using UIFrame;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        /// 预加载进度回调，参数为 0.0 ~ 1.0
        /// </summary>
        public event Action<float> OnPreloadProgress;

        /// <summary>
        /// 预加载完成回调
        /// </summary>
        public event Action OnPreloadCompleted;

        /// <summary>
        /// 搭建Scene场景，后续完善
        /// </summary>
        public virtual void BuildGameScene()
        {
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
        /// </summary>
        protected Dictionary<string, PawnInfoData> m_PawnInfo = new Dictionary<string, PawnInfoData>();
        protected string m_PawnInfoName = "PawnInfo";

        /// <summary>
        /// 将Effect的数据存成字典，方便后续读取
        /// </summary>
        protected Dictionary<string, EffectInfoData> m_EffectInfo = new Dictionary<string, EffectInfoData>();
        protected string m_EffectInfoName = "EffectInfo";

        /// <summary>
        /// 将AssetId文件的数据存成字典，方便后续读取
        /// </summary>
        protected Dictionary<string, AssetIDData> m_AssetId = new Dictionary<string, AssetIDData>();
        protected string m_AssetIdName = "AssetId";

        /// <summary>
        /// 将ItemInfo文件的数据存成字典，方便后续读取
        /// </summary>
        protected Dictionary<string, ItemInfoData> m_ItemInfo = new Dictionary<string, ItemInfoData>();
        protected string m_ItemInfoName = "ItemInfo";

        /// <summary>
        /// 标记预加载是否已完成
        /// </summary>
        public bool IsPreloadCompleted { get; private set; } = false;

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
            DontDestroyOnLoad(this);
        }

        #region 预加载系统（游戏启动后、PersistentScene中调用）

        /// <summary>
        /// 在指定场景中创建Pool根节点和所有层级容器
        /// 创建的对象会通过 MoveGameObjectToScene 归入 targetScene
        /// </summary>
        public void InitPoolHierarchyInScene(Scene targetScene)
        {
            Scene previousActive = SceneManager.GetActiveScene();

            // 切换到目标场景以便 new GameObject 直接创建在该场景中
            SceneManager.SetActiveScene(targetScene);

            if (PoolRoot == null)
            {
                PoolRoot = new GameObject("PoolRoot").transform;
            }

            InstancesPoolLevel = CreatePoolLevelInScene("InstancesPoolLevel", targetScene);
            BoolCanPlaceFumiturePoolLevel = CreatePoolLevelInScene("BoolCanPlaceFumiturePoolLevel", targetScene);
            BoolCanFropItemPoolLevel = CreatePoolLevelInScene("BoolCanFropItemPoolLevel", targetScene);
            BoolDiggablePoolLevel = CreatePoolLevelInScene("BoolDiggablePoolLevel", targetScene);
            BoolPathPoolLevel = CreatePoolLevelInScene("BoolPathPoolLevel", targetScene);
            BoolNpcObstaciePoolLevel = CreatePoolLevelInScene("BoolNpcObstaciePoolLevel", targetScene);

            // 恢复之前的激活场景
            SceneManager.SetActiveScene(previousActive);

            Debug.Log($"[GameBuilder] Pool层级已在场景'{targetScene.name}'中创建完成");
        }

        /// <summary>
        /// 在指定场景中创建池层级容器，并挂接到PoolRoot
        /// </summary>
        private Transform CreatePoolLevelInScene(string name, Scene scene)
        {
            GameObject obj = new GameObject(name);
            SceneManager.MoveGameObjectToScene(obj, scene);
            if (PoolRoot != null)
            {
                obj.transform.SetParent(PoolRoot.transform);
                return obj.transform;
            }
            throw new UIFrameException("PoolRoot不存在，请先调用InitPoolHierarchyInScene");
        }

        /// <summary>
        /// 读取CSV配置并创建对象池（使用已建立的PoolHierarchy）
        /// </summary>
        public void LoadAndSetupPools()
        {
            IsPreloadCompleted = false;
            ReportProgress(0f);

            // 读取所有CSV配置数据（0% → 50%）
            LoadCsvData();

            // 为每个AssetId创建对象池（50% → 90%）
            SetupPoolsFromAssetId();

            // 完成（90% → 100%）
            ReportProgress(1f);
            IsPreloadCompleted = true;
            OnPreloadCompleted?.Invoke();
            Debug.Log("[GameBuilder] 预加载完成");
        }

        /// <summary>
        /// 读取所有CSV配置数据到内存字典
        /// </summary>
        private void LoadCsvData()
        {
            m_AssetId = FileManager.Instance.ReadCSVFilesToDictionary<AssetIDData>(m_AssetIdName);
            ReportProgress(0.2f);

            m_PawnInfo = FileManager.Instance.ReadCSVFilesToDictionary<PawnInfoData>(m_PawnInfoName);
            ReportProgress(0.3f);

            m_EffectInfo = FileManager.Instance.ReadCSVFilesToDictionary<EffectInfoData>(m_EffectInfoName);
            ReportProgress(0.4f);

            m_ItemInfo = FileManager.Instance.ReadCSVFilesToDictionary<ItemInfoData>(m_ItemInfoName);
            ReportProgress(0.5f);
        }

        /// <summary>
        /// 根据AssetID配置创建并预热对象池
        /// </summary>
        private void SetupPoolsFromAssetId()
        {
            if (m_AssetId == null || m_AssetId.Count == 0)
            {
                Debug.LogWarning("[GameBuilder] AssetID数据为空，跳过池创建");
                ReportProgress(0.9f);
                return;
            }

            int total = m_AssetId.Count;
            int processed = 0;

            foreach (var entry in m_AssetId)
            {
                AssetIDData data = entry.Value;

                // 加载预制体
                GameObject prefab = Resources.Load<GameObject>(data.assetPath);
                if (prefab == null)
                {
                    Debug.LogWarning($"[GameBuilder] 资源未找到，跳过: {data.assetPath}");
                    processed++;
                    continue;
                }

                // 确定层级
                int assetType;
                int.TryParse(data.type, out assetType);
                Transform poolParent = GetPoolLevelByType(assetType);

                // 解析尺寸
                int initialSize;
                int.TryParse(data.initialSize, out initialSize);
                int maxSize;
                int.TryParse(data.maxSize, out maxSize);

                // 添加池配置（不重复添加）
                ObjectPool.AddPool(data.id, prefab, poolParent, initialSize, maxSize);
                processed++;

                // 进度：50% → 90% 之间按已处理比例推进
                ReportProgress(0.5f + 0.4f * processed / total);
            }

            ObjectPool.InitializePools();
        }

        /// <summary>
        /// 报告预加载进度
        /// </summary>
        private void ReportProgress(float progress)
        {
            OnPreloadProgress?.Invoke(Mathf.Clamp01(progress));
        }

        /// <summary>
        /// 获取池层级（使用InitPoolHierarchy创建的层级节点）
        /// </summary>
        private Transform GetPoolLevelByType(int level)
        {
            switch (level)
            {
                case 1: return InstancesPoolLevel;
                case 2: return BoolCanPlaceFumiturePoolLevel;
                case 3: return BoolCanFropItemPoolLevel;
                case 4: return BoolDiggablePoolLevel;
                case 5: return BoolPathPoolLevel;
                case 6: return BoolNpcObstaciePoolLevel;
                default: return null;
            }
        }

        #endregion

        #region 加载场景
        #endregion

        #region 数据初始化
        /// <summary>
        /// 场景加载完成后初始化数据
        /// 仅在预加载未完成时读取CSV；已预加载则跳过CSV读取，只初始化池
        /// </summary>
        public void LoadAssetIDData()
        {
            if (IsPreloadCompleted)
            {
                if (!ObjectPool.IsInitialized)
                {
                    ObjectPool.InitializePools();
                }
                return;
            }

            // 未经过预加载时的后备初始化
            m_AssetId = FileManager.Instance.ReadCSVFilesToDictionary<AssetIDData>(m_AssetIdName);
            m_PawnInfo = FileManager.Instance.ReadCSVFilesToDictionary<PawnInfoData>(m_PawnInfoName);
            m_EffectInfo = FileManager.Instance.ReadCSVFilesToDictionary<EffectInfoData>(m_EffectInfoName);
            m_ItemInfo = FileManager.Instance.ReadCSVFilesToDictionary<ItemInfoData>(m_ItemInfoName);

            if (m_AssetId != null)
            {
                foreach (var entry in m_AssetId)
                {
                    AssetIDData data = entry.Value;
                    GameObject prefab = Resources.Load<GameObject>(data.assetPath);
                    if (prefab == null) continue;
                    int assetType;
                    int.TryParse(data.type, out assetType);
                    int initialSize;
                    int.TryParse(data.initialSize, out initialSize);
                    int maxSize;
                    int.TryParse(data.maxSize, out maxSize);
                    ObjectPool.AddPool(data.id, prefab, GetPoolLevelByType(assetType), initialSize, maxSize);
                }
                ObjectPool.InitializePools();
            }
        }

        /// <summary>
        /// 获取Map场景中的对象层级（TilemapGrid下的层级）
        /// </summary>
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
        public ItemInfoData ContainsItemInfo(string assetId)
        {
            if (m_ItemInfo.TryGetValue(assetId, out ItemInfoData item))
            {
                return item;
            }
            Debug.LogError($"ContainsItemInfo: assetId '{assetId}' doesn't exist.");
            return null;
        }

        /// <summary>
        /// 通过assetId查找对应的ItemDetails
        /// </summary>
        public ItemDetails ContainsItemDetails(string assetId)
        {
            ItemInfoData itemInfoData = ContainsItemInfo(assetId);

            if (itemInfoData != null)
            {
                return new ItemDetails(itemInfoData);
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

