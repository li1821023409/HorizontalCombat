using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor.VersionControl;
using UnityEngine;
using static WNGameBase.ObjectPool;

namespace WNGameBase
{
    /// <summary>
    /// 属于基层，如果后续有热更代码，请勿轻易热更基础代码
    /// </summary>
    public class ObjectPool : UnitySingleton<ObjectPool>
    {
        [System.Serializable]
        public class Pool
        {
            public string assetId;
            // TODO：这里稍后改成路径，之后创建通过路径创建吧，哎，忘记了，以后改一下
            public GameObject prefab;
            public int initialSize = 10;
            [Tooltip("0 for unlimited")]
            public int maxSize = 0;
            [HideInInspector] public Transform poolParent;
        }

        [System.Serializable]
        public class PoolStats
        {
            public string assetId;
            public int totalObjects;
            public int activeObjects;
            public int availableObjects;
            public int peakUsage;
            public float utilizationPercent => totalObjects > 0 ? (float)activeObjects / totalObjects * 100f : 0f;
            public float peakUtilizationPercent => totalObjects > 0 ? (float)peakUsage / totalObjects * 100f : 0f;
        }

        // 单例模式，方便全局访问
        //public static ObjectPool Instance { get; private set; }

        [SerializeField] private bool createPoolsOnAwake = true;
        [SerializeField] private List<Pool> pools = new List<Pool>();

        // 对象池数据结构
        private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // 用于跟踪统计信息的字典
        private Dictionary<string, int> activeCountByAssetId = new Dictionary<string, int>();
        private Dictionary<string, int> peaks = new Dictionary<string, int>();

        public Pool AddPool(string m_assetId, GameObject m_prefab, Transform m_poolParent = null, int m_initialSize = 1, int m_maxSize = 1)
        {
            Pool pool = new Pool();
            pool.assetId = m_assetId;
            pool.prefab = m_prefab;
            pool.initialSize = m_initialSize;
            pool.maxSize = m_maxSize;
            pool.poolParent = m_poolParent;
            pools.Add(pool);

            return pool;
        }

        /// <summary>
        /// 初始化所有对象池
        /// </summary>
        public void InitializePools()
        {
            poolDictionary.Clear();
            activeCountByAssetId.Clear();
            peaks.Clear();

            foreach (Pool pool in pools)
            {
                if (string.IsNullOrEmpty(pool.assetId) || pool.prefab == null)
                {
                    Debug.LogError("Pool configuration error: AssetId cannot be empty and prefab must be assigned");
                    continue;
                }

                // 创建父物体用于组织层级
                //GameObject poolContainer = new GameObject($"Pool_{pool.assetId}");
                //poolContainer.transform.SetParent(transform);
                //pool.poolParent = poolContainer.transform;

                InitPool(pool); 
            }
        }

        public void InitPool(Pool pool)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // 预先实例化对象
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = CreateNewPoolObject(pool);
                objectPool.Enqueue(obj);
            }

            // 添加到字典
            poolDictionary.Add(pool.assetId, objectPool);
            activeCountByAssetId.Add(pool.assetId, 0);
            peaks.Add(pool.assetId, 0);

            Debug.Log($"Pool '{pool.assetId}' initialized with {pool.initialSize} objects");
        }

        public bool PoolContains(string assetId)
        {
            return poolDictionary.ContainsKey(assetId);
        }

        /// <summary>
        /// 创建池对象并添加到池中
        /// </summary>
        public GameObject CreateAndAddPoolObject(string m_assetId, string path, Vector3 position, Quaternion rotation, Transform m_poolParent = null, int m_initialSize = 1, int m_maxSize = 1)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                return null;
            }
            Pool pool = AddPool(m_assetId, prefab, m_poolParent, m_initialSize, m_maxSize);
            InitPool(pool);
            return SpawnFromPool(m_assetId, position, rotation, m_poolParent);
        }

        /// <summary>
        /// 创建新的池对象
        /// </summary>
        private GameObject CreateNewPoolObject(Pool pool)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            obj.transform.SetParent(pool.poolParent);

            //// 添加PooledObject组件以便跟踪
            //PooledObject pooledObj = obj.GetComponent<PooledObject>();
            //if (pooledObj == null)
            //{
            //    pooledObj = obj.AddComponent<PooledObject>();
            //}
            //pooledObj.SetPool(this, pool.assetId);

            return obj;
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public GameObject SpawnFromPool(string assetId, Vector3 position, Quaternion rotation, Transform transform = null)
        {
            if (!poolDictionary.ContainsKey(assetId))
            {
                Debug.LogWarning($"Pool with assetId '{assetId}' doesn't exist.");
                return null;
            }

            // 更新活跃对象计数
            activeCountByAssetId[assetId]++;

            // 更新峰值使用量
            if (activeCountByAssetId[assetId] > peaks[assetId])
            {
                peaks[assetId] = activeCountByAssetId[assetId];
            }

            // 获取对象队列
            Queue<GameObject> objectPool = poolDictionary[assetId];

            // 如果池为空，尝试创建新对象
            if (objectPool.Count == 0)
            {
                Pool poolInfo = pools.Find(p => p.assetId == assetId);

                // 检查是否达到最大容量限制
                if (poolInfo.maxSize > 0 && activeCountByAssetId[assetId] >= poolInfo.maxSize)
                {
                    Debug.LogWarning($"Pool '{assetId}' has reached its maximum size ({poolInfo.maxSize}). Cannot create more objects.");
                    activeCountByAssetId[assetId]--; // 恢复计数
                    return null;
                }

                GameObject newObj = CreateNewPoolObject(poolInfo);
                Debug.Log($"Pool '{assetId}' expanded with one new object. Total: {activeCountByAssetId[assetId] + objectPool.Count}");
                return SetupPooledObject(newObj, position, rotation);
            }

            // 取出并设置对象
            GameObject pooledObject = objectPool.Dequeue();

            // 检查对象是否已被销毁（例如在场景切换期间）
            if (pooledObject == null)
            {
                Pool poolInfo = pools.Find(p => p.assetId == assetId);
                pooledObject = CreateNewPoolObject(poolInfo);
                Debug.LogWarning($"Object in pool '{assetId}' was destroyed. Created a new one.");
            }

            return SetupPooledObject(pooledObject, position, rotation);
        }

        /// <summary>
        /// 设置池对象的位置、旋转并激活
        /// </summary>
        private GameObject SetupPooledObject(GameObject obj, Vector3 position, Quaternion rotation)
        {
            // 设置变换
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            // 激活对象
            obj.SetActive(true);

            // 获取IPoolable接口并重置
            IPoolable poolable = obj.GetComponent<IPoolable>();
            if (poolable != null)
            {
                poolable.OnObjectSpawn();
            }

            return obj;
        }

        /// <summary>
        /// 返回对象到池
        /// </summary>
        public void ReturnToPool(string assetId, GameObject obj)
        {
            if (!poolDictionary.ContainsKey(assetId))
            {
                Debug.LogWarning($"Trying to return an object to non-existent pool '{assetId}'");
                return;
            }

            // 更新活跃对象计数
            activeCountByAssetId[assetId] = Mathf.Max(0, activeCountByAssetId[assetId] - 1);

            // 重置对象状态
            obj.SetActive(false);

            // 获取IPoolable接口并重置
            IPoolable poolable = obj.GetComponent<IPoolable>();
            if (poolable != null)
            {
                poolable.OnObjectReturn();
            }

            // 返回到池
            poolDictionary[assetId].Enqueue(obj);
        }

        /// <summary>
        /// 获取池统计信息
        /// </summary>
        public List<PoolStats> GetPoolStats()
        {
            List<PoolStats> stats = new List<PoolStats>();

            foreach (var poolEntry in poolDictionary)
            {
                string assetId = poolEntry.Key;
                Queue<GameObject> pool = poolEntry.Value;

                int active = activeCountByAssetId.ContainsKey(assetId) ? activeCountByAssetId[assetId] : 0;
                int available = pool.Count;
                int total = active + available;
                int peak = peaks.ContainsKey(assetId) ? peaks[assetId] : 0;

                stats.Add(new PoolStats
                {
                    assetId = assetId,
                    totalObjects = total,
                    activeObjects = active,
                    availableObjects = available,
                    peakUsage = peak
                });
            }

            return stats;
        }

        /// <summary>
        /// 预热池 - 增加池的大小
        /// </summary>
        public void PrewarmPool(string assetId, int additionalCount)
        {
            if (!poolDictionary.ContainsKey(assetId))
            {
                Debug.LogWarning($"Cannot prewarm non-existent pool '{assetId}'");
                return;
            }

            Pool poolInfo = pools.Find(p => p.assetId == assetId);
            if (poolInfo == null) return;

            // 检查最大大小限制
            int currentTotal = poolDictionary[assetId].Count + activeCountByAssetId[assetId];
            if (poolInfo.maxSize > 0 && currentTotal + additionalCount > poolInfo.maxSize)
            {
                additionalCount = Mathf.Max(0, poolInfo.maxSize - currentTotal);

                if (additionalCount == 0)
                {
                    Debug.LogWarning($"Cannot prewarm pool '{assetId}' further: already at maximum size ({poolInfo.maxSize})");
                    return;
                }
            }

            // 创建额外对象
            for (int i = 0; i < additionalCount; i++)
            {
                GameObject obj = CreateNewPoolObject(poolInfo);
                poolDictionary[assetId].Enqueue(obj);
            }

            Debug.Log($"Prewarmed '{assetId}' pool with {additionalCount} additional objects. Total: {poolDictionary[assetId].Count + activeCountByAssetId[assetId]}");
        }

        /// <summary>
        /// 清空池
        /// </summary>
        public void ClearPool(string assetId)
        {
            if (!poolDictionary.ContainsKey(assetId))
            {
                Debug.LogWarning($"Cannot clear non-existent pool '{assetId}'");
                return;
            }

            Queue<GameObject> pool = poolDictionary[assetId];
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                if (obj != null)
                {
                    Destroy(obj);
                }
            }

            Debug.Log($"Pool '{assetId}' cleared");
        }

        /// <summary>
        /// 清空所有池
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var assetId in poolDictionary.Keys.ToList())
            {
                ClearPool(assetId);
            }

            poolDictionary.Clear();
            activeCountByAssetId.Clear();
            peaks.Clear();

            Debug.Log("All pools cleared");
        }

        /// <summary>
        /// 重置峰值统计
        /// </summary>
        public void ResetPeakStats(string assetId = null)
        {
            if (assetId != null)
            {
                if (peaks.ContainsKey(assetId))
                    peaks[assetId] = activeCountByAssetId.ContainsKey(assetId) ? activeCountByAssetId[assetId] : 0;
            }
            else
            {
                // 重置所有池的峰值
                foreach (var key in peaks.Keys.ToList())
                {
                    peaks[key] = activeCountByAssetId.ContainsKey(key) ? activeCountByAssetId[key] : 0;
                }
            }
        }

        /// <summary>
        /// 基于峰值使用情况优化池大小
        /// </summary>
        public void OptimizePoolSizes()
        {
            List<PoolStats> stats = GetPoolStats();

            foreach (var stat in stats)
            {
                // 如果峰值使用量接近总容量，增加池大小
                if (stat.peakUtilizationPercent > 90f)
                {
                    int additionalSize = Mathf.CeilToInt(stat.totalObjects * 0.2f); // 增加20%
                    PrewarmPool(stat.assetId, additionalSize);
                }
                // 如果峰值使用量远低于容量，记录建议
                else if (stat.peakUtilizationPercent < 40f && stat.totalObjects > 20)
                {
                    int suggestedSize = Mathf.CeilToInt(stat.peakUsage * 1.5f); // 峰值的1.5倍
                    Debug.Log($"Pool '{stat.assetId}' might be oversized. Current size: {stat.totalObjects}, Suggested size: {suggestedSize}");
                }
            }
        }
    }

    /// <summary>
    /// 标记可池化对象的接口
    /// </summary>
    public interface IPoolable
    {
        void OnObjectSpawn();
        void OnObjectReturn();
    }

    /// <summary>
    /// 为池对象提供自动返回功能的组件
    /// </summary>
    //public class PooledObject : MonoBehaviour
    //{
    //    private ObjectPool pool;
    //    private string poolAssetId;

    //    public void SetPool(ObjectPool objectPool, string assetId)
    //    {
    //        pool = objectPool;
    //        poolAssetId = assetId;
    //    }

    //    public void ReturnToPool()
    //    {
    //        if (pool != null)
    //        {
    //            pool.ReturnToPool(poolAssetId, gameObject);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Pool reference is missing. Cannot return object.", gameObject);
    //        }
    //    }
    //}
}