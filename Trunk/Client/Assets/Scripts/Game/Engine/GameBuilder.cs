using FileIO;
using System.Collections.Generic;
using UIFrame;
using UnityEngine;
using WNGameBase;

namespace WNEngine
{
    public enum AttackableTargetLevel
    {
        None,
        LocalPlayer,        // 本地玩家
        FriendlyForces,     // 友军
        Enemy,              // 敌人
        SceneObject,        // 场景对象
    }

    public class GameBuilder
    {
        #region 基础数据
        ///// <summary>
        ///// PlayerInfo类型
        ///// </summary>
        //public System.Type PlayerInfoType;

        ///// <summary>
        ///// UIScene类型
        ///// </summary>
        //public System.Type UISceneType;

        /// <summary>
        /// 搭建UIScene场景，后续完善
        /// </summary>
        //public virtual UIScene BuildGameUIScene()
        //{
        //    return ReflectUtility.CreateInstance(UISceneType) as UIScene;
        //}

        /// <summary>
        /// 创建gameInfo，后续完善
        /// </summary>
        //public virtual void BuildGameInfo(GameInfo gameInfo) { DoBuildGameInfo(gameInfo); }

        //protected virtual void DoBuildGameInfo(GameInfo gameInfo) { }

        /// <summary>
        /// pawn根节点
        /// </summary>
        private Transform pawnRoot;
        /// <summary>
        /// 界面层级节点，None
        /// </summary>
        private Transform NoneLevel;
        /// <summary>
        /// 界面层级节点，LocalPlayer
        /// </summary>
        private Transform LocalPlayerLevel;
        /// <summary>
        /// 界面层级节点，FriendlyForces
        /// </summary>
        private Transform FriendlyForcesLevel;
        /// <summary>
        /// 界面层级节点，Enemy
        /// </summary>
        private Transform EnemyLevel;
        /// <summary>
        /// 界面层级节点，SceneObject
        /// </summary>
        private Transform SceneObjectLevel;

        protected ObjectPool objectPool = ObjectPool.Instance;

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
                    objectPool.AddPool(data.id, prefab, GetAttackableTargetLevel(int.Parse(data.assetType)), int.Parse(data.initialSize), int.Parse(data.maxSize));
                }
                //else
                //{
                //    Debug.LogError($"AssetIDData with assetId '{key}' doesn't exist.");
                //}
            }
            objectPool.InitializePools();

        }

        private void InitPawnLevel()
        {
            if (pawnRoot == null)
            {
                GameObject obj = new GameObject("PawnRoot");
                pawnRoot = obj.transform;
            }

            NoneLevel = CreateAttackableTargetLevel(GetAttackableTargetLevelName(AttackableTargetLevel.None), (int)AttackableTargetLevel.None);
            LocalPlayerLevel = CreateAttackableTargetLevel(GetAttackableTargetLevelName(AttackableTargetLevel.LocalPlayer), (int)AttackableTargetLevel.LocalPlayer);
            FriendlyForcesLevel = CreateAttackableTargetLevel(GetAttackableTargetLevelName(AttackableTargetLevel.FriendlyForces), (int)AttackableTargetLevel.FriendlyForces);
            EnemyLevel = CreateAttackableTargetLevel(GetAttackableTargetLevelName(AttackableTargetLevel.Enemy), (int)AttackableTargetLevel.Enemy);
            SceneObjectLevel = CreateAttackableTargetLevel(GetAttackableTargetLevelName(AttackableTargetLevel.SceneObject), (int)AttackableTargetLevel.SceneObject);
        }

        /// <summary>
        /// 创建可攻击对象层级，对Pawn进行分层存放
        /// </summary>
        private Transform CreateAttackableTargetLevel(string name, int level)
        {
            GameObject obj = new GameObject(name);
            if (pawnRoot != null)
            {
                obj.transform.SetParent(pawnRoot.transform);
                return obj.transform;
            }
            else
            {
                throw new UIFrameException("pawnToot不存在");
            }
        }

        /// <summary>
        /// 获取可攻击对象层级名称
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetAttackableTargetLevelName(AttackableTargetLevel level)
        {
            string name = null;
            switch (level)
            {
                case AttackableTargetLevel.None:
                    name = "None";
                    break;
                case AttackableTargetLevel.LocalPlayer:
                    name = "LocalPlayer";
                    break;
                case AttackableTargetLevel.FriendlyForces:
                    name = "FriendlyForces";
                    break;
                case AttackableTargetLevel.Enemy:
                    name = "Enemy";
                    break;
                case AttackableTargetLevel.SceneObject:
                    name = "SceneObject";
                    break;
                default:
                    break;
            }
            return name;
        }

        /// <summary>
        /// 获取可攻击对象层级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private Transform GetAttackableTargetLevel(int level)
        {
            switch (level)
            {
                case 0:
                    return NoneLevel;
                case 1:
                    return LocalPlayerLevel;
                case 2:
                    return FriendlyForcesLevel;
                case 3:
                    return EnemyLevel;
                case 4:
                    return SceneObjectLevel;
                default:
                    return NoneLevel;
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

                pawnName = ObjectPool.Instance.SpawnFromPool(assetId, position, rotation);

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
                int.TryParse(pawnInfoData.assetType, out pawnInfo.assetType);
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
                ObjectPool.Instance.ReturnToPool(pawn.m_PawnInfo.id, pawn.gameObject);
            }
        }
        #endregion

        #region Effects对象池化处理
        /// <summary>
        /// 创建Effect对象
        /// </summary>
        public void SpawnEffect(string assetId, Vector3 position, Quaternion rotation)
        {
            EffectInfoData effectInfo = ContainsEffectInfo(assetId);

            // pawnInfo 为空说明配置有问题

            if (effectInfo != null)
            {
                GameObject effectName = null;
                effectName = ObjectPool.Instance.SpawnFromPool(assetId, position, rotation);

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
        /// 创建Item对象
        /// </summary>
        public GameObject SpawnItem(string assetId, Vector3 position, Quaternion rotation, Transform parent = null, int initialSize = 1, int maxSize = 1)
        {
            GameObject item = null;

            if (ObjectPool.Instance.PoolContains(assetId))
            {
                item = ObjectPool.Instance.SpawnFromPool(assetId, position, rotation);
            }
            else
            {
                // 没有获取说明没有添加该对象到池中，这里添加一下（也可以不添加，后面做一下处理）
                ItemInfoData data = m_ItemInfo[assetId];
                item = ObjectPool.Instance.CreateAndAddPoolObject(assetId, data.itemPath, position, rotation, parent);
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
        /// pawn死亡，返回对象池
        /// </summary>
        public void DestroyItem(string itemId, GameObject item)
        {
            if (!string.IsNullOrEmpty(itemId) && item != null)
            {
                // 返回对象池
                ObjectPool.Instance.ReturnToPool(itemId, item);
            }
        }
        #endregion
    }
}

