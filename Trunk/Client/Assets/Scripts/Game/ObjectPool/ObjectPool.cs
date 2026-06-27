using System.Collections.Generic;
using UnityEngine;

namespace WNGameBase
{
    /// <summary>
    /// 对象池管理器 - 属于基层，如果后续有热更代码，请勿轻易热更基础代码
    /// </summary>
    public class ObjectPool : UnitySingleton<ObjectPool>
    {
        [System.Serializable]
        public class Pool
        {
            public string assetId;
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

        /// <summary>
        /// 池运行时数据，将队列、计数与配置引用整合为单一结构
        /// </summary>
        private class PoolRuntime
        {
            public Pool Config;
            public readonly Queue<GameObject> Available = new Queue<GameObject>();
            public int ActiveCount;
            public int PeakUsage;

            public int TotalCount => ActiveCount + Available.Count;

            /// <summary>记录一个活跃对象并更新峰值</summary>
            public void RecordActive()
            {
                ActiveCount++;
                if (ActiveCount > PeakUsage)
                    PeakUsage = ActiveCount;
            }

            /// <summary>释放一个活跃对象</summary>
            public void ReleaseActive()
            {
                if (ActiveCount > 0)
                    ActiveCount--;
            }
        }

        [SerializeField] private List<Pool> pools = new List<Pool>();

        private readonly Dictionary<string, PoolRuntime> runtimeMap = new Dictionary<string, PoolRuntime>();

        /// <summary>
        /// 池是否已完成初始化
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// 添加池配置
        /// </summary>
        public Pool AddPool(string assetId, GameObject prefab, Transform poolParent = null, int initialSize = 1, int maxSize = 1)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                Debug.LogError("Cannot add pool: assetId is null or empty");
                return null;
            }

            if (prefab == null)
            {
                Debug.LogError("Cannot add pool: prefab is null");
                return null;
            }

            if (runtimeMap.ContainsKey(assetId) || pools.Exists(p => p.assetId == assetId))
            {
                Debug.LogWarning($"Pool '{assetId}' already exists");
                return null;
            }

            Pool pool = new Pool
            {
                assetId = assetId,
                prefab = prefab,
                initialSize = initialSize,
                maxSize = maxSize,
                poolParent = poolParent
            };

            pools.Add(pool);
            return pool;
        }

        /// <summary>
        /// 初始化所有对象池
        /// </summary>
        public void InitializePools()
        {
            runtimeMap.Clear();

            foreach (Pool pool in pools)
            {
                if (string.IsNullOrEmpty(pool.assetId) || pool.prefab == null)
                {
                    Debug.LogError("Pool configuration error: AssetId cannot be empty and prefab must be assigned");
                    continue;
                }

                InitializePool(pool);
            }

            IsInitialized = true;
        }

        private void InitializePool(Pool pool)
        {
            if (runtimeMap.ContainsKey(pool.assetId))
            {
                Debug.LogWarning($"Pool '{pool.assetId}' is already initialized, skipping");
                return;
            }

            var runtime = new PoolRuntime { Config = pool };

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = InstantiatePoolObject(pool);
                runtime.Available.Enqueue(obj);
            }

            runtimeMap.Add(pool.assetId, runtime);
            Debug.Log($"Pool '{pool.assetId}' initialized with {pool.initialSize} objects");
        }

        /// <summary>
        /// 检查指定池是否存在
        /// </summary>
        public bool PoolContains(string assetId)
        {
            return runtimeMap.ContainsKey(assetId);
        }

        /// <summary>
        /// 创建池对象并添加到池中
        /// </summary>
        public GameObject CreateAndAddPoolObject(string assetId, string path, Vector3 position, Quaternion rotation, Transform poolParent = null, int initialSize = 1, int maxSize = 1)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab at path '{path}'");
                return null;
            }

            Pool pool = AddPool(assetId, prefab, poolParent, initialSize, maxSize);
            if (pool == null) return null;

            InitializePool(pool);
            return SpawnFromPool(assetId, position, rotation, poolParent);
        }

        /// <summary>
        /// 实例化新的池对象（不激活）
        /// </summary>
        private GameObject InstantiatePoolObject(Pool pool)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            obj.transform.SetParent(pool.poolParent);
            return obj;
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public GameObject SpawnFromPool(string assetId, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (!runtimeMap.TryGetValue(assetId, out PoolRuntime runtime))
            {
                Debug.LogWarning($"Pool with assetId '{assetId}' doesn't exist.");
                return null;
            }

            runtime.RecordActive();

            GameObject obj;

            if (runtime.Available.Count > 0)
            {
                obj = runtime.Available.Dequeue();

                // 对象可能在场景切换期间被销毁
                if (obj == null)
                {
                    obj = InstantiatePoolObject(runtime.Config);
                    Debug.LogWarning($"Object in pool '{assetId}' was destroyed. Created a new one.");
                }
            }
            else
            {
                // 超出最大容量时仍创建对象，但归还时将丢弃溢出对象
                if (runtime.Config.maxSize > 0 && runtime.ActiveCount > runtime.Config.maxSize)
                {
                    Debug.LogWarning($"Pool '{assetId}' exceeded max capacity ({runtime.Config.maxSize}). Creating overflow object. Active: {runtime.ActiveCount}");
                }

                obj = InstantiatePoolObject(runtime.Config);
                Debug.Log($"Pool '{assetId}' expanded with one new object. Total: {runtime.TotalCount}");
            }

            return ActivateObject(obj, position, rotation, parent);
        }

        /// <summary>
        /// 设置对象变换并激活，调用 IPoolable.OnObjectSpawn
        /// </summary>
        private GameObject ActivateObject(GameObject obj, Vector3 position, Quaternion rotation, Transform parent)
        {
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            if (obj.TryGetComponent<IPoolable>(out var poolable))
                poolable.OnObjectSpawn();

            return obj;
        }

        /// <summary>
        /// 返回对象到池
        /// </summary>
        public void ReturnToPool(string assetId, GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogWarning($"Trying to return a null object to pool '{assetId}'");
                return;
            }

            if (!runtimeMap.TryGetValue(assetId, out PoolRuntime runtime))
            {
                Debug.LogWarning($"Trying to return an object to non-existent pool '{assetId}'");
                return;
            }

            runtime.ReleaseActive();

            if (obj.TryGetComponent<IPoolable>(out var poolable))
                poolable.OnObjectReturn();

            // 池内对象总数已达最大容量，销毁溢出对象
            if (runtime.Config.maxSize > 0 && runtime.TotalCount >= runtime.Config.maxSize)
            {
                Destroy(obj);
                Debug.Log($"Pool '{assetId}' at max capacity ({runtime.Config.maxSize}). Discarded overflow object.");
                return;
            }

            obj.transform.SetParent(runtime.Config.poolParent);
            obj.SetActive(false);
            runtime.Available.Enqueue(obj);
        }

        /// <summary>
        /// 获取池统计信息
        /// </summary>
        public List<PoolStats> GetPoolStats()
        {
            var stats = new List<PoolStats>(runtimeMap.Count);

            foreach (var entry in runtimeMap)
            {
                PoolRuntime runtime = entry.Value;
                stats.Add(new PoolStats
                {
                    assetId = entry.Key,
                    totalObjects = runtime.TotalCount,
                    activeObjects = runtime.ActiveCount,
                    availableObjects = runtime.Available.Count,
                    peakUsage = runtime.PeakUsage
                });
            }

            return stats;
        }

        /// <summary>
        /// 预热池 - 增加池的大小
        /// </summary>
        public void PrewarmPool(string assetId, int additionalCount)
        {
            if (additionalCount <= 0)
            {
                Debug.LogWarning($"Prewarm count must be positive, got {additionalCount}");
                return;
            }

            if (!runtimeMap.TryGetValue(assetId, out PoolRuntime runtime))
            {
                Debug.LogWarning($"Cannot prewarm non-existent pool '{assetId}'");
                return;
            }

            // 检查最大大小限制
            if (runtime.Config.maxSize > 0)
            {
                int remaining = runtime.Config.maxSize - runtime.TotalCount;
                if (remaining <= 0)
                {
                    Debug.LogWarning($"Cannot prewarm pool '{assetId}' further: already at maximum size ({runtime.Config.maxSize})");
                    return;
                }

                if (additionalCount > remaining)
                    additionalCount = remaining;
            }

            for (int i = 0; i < additionalCount; i++)
            {
                GameObject obj = InstantiatePoolObject(runtime.Config);
                runtime.Available.Enqueue(obj);
            }

            Debug.Log($"Prewarmed '{assetId}' pool with {additionalCount} additional objects. Total: {runtime.TotalCount}");
        }

        /// <summary>
        /// 清空指定池中的可用对象
        /// </summary>
        public void ClearPool(string assetId)
        {
            if (!runtimeMap.TryGetValue(assetId, out PoolRuntime runtime))
            {
                Debug.LogWarning($"Cannot clear non-existent pool '{assetId}'");
                return;
            }

            DestroyAvailableObjects(runtime);
            Debug.Log($"Pool '{assetId}' cleared");
        }

        /// <summary>
        /// 清空所有池
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var entry in runtimeMap)
                DestroyAvailableObjects(entry.Value);

            runtimeMap.Clear();
            Debug.Log("All pools cleared");
        }

        /// <summary>
        /// 重置峰值统计
        /// </summary>
        public void ResetPeakStats(string assetId = null)
        {
            if (assetId != null)
            {
                if (runtimeMap.TryGetValue(assetId, out PoolRuntime runtime))
                    runtime.PeakUsage = runtime.ActiveCount;
            }
            else
            {
                foreach (var entry in runtimeMap)
                    entry.Value.PeakUsage = entry.Value.ActiveCount;
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
                if (stat.peakUtilizationPercent > 90f)
                {
                    int additionalSize = Mathf.CeilToInt(stat.totalObjects * 0.2f);
                    PrewarmPool(stat.assetId, additionalSize);
                }
                else if (stat.peakUtilizationPercent < 40f && stat.totalObjects > 20)
                {
                    int suggestedSize = Mathf.CeilToInt(stat.peakUsage * 1.5f);
                    Debug.Log($"Pool '{stat.assetId}' might be oversized. Current size: {stat.totalObjects}, Suggested size: {suggestedSize}");
                }
            }
        }

        /// <summary>
        /// 销毁池中所有可用对象
        /// </summary>
        private void DestroyAvailableObjects(PoolRuntime runtime)
        {
            while (runtime.Available.Count > 0)
            {
                GameObject obj = runtime.Available.Dequeue();
                if (obj != null)
                    Destroy(obj);
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
}
