using System.Collections.Generic;
using UnityEngine;

namespace UIFrame
{
    public class ResMgr : UnitySingleton<ResMgr>
    {
        private Dictionary<string, GameObject> assetCache = new Dictionary<string, GameObject>();

        /// <summary>  
        /// 从缓存中获取资源，如果不存在则从资源文件中加载并缓存  
        /// </summary>  
        /// <param name="path">资源路径</param>  
        /// <returns>GameObject 资源</returns>  
        public T GetAssetCache<T>(string path) where T : UnityEngine.Object
        {
            // 检查缓存中是否存在  
            if (assetCache.TryGetValue(path, out var cachedAsset))
            {
                return cachedAsset as T;
            }

            // 从资源文件中加载  
            T asset = Resources.Load<T>(path);
            if (asset != null)
            {
                // 添加到缓存  
                assetCache[path] = asset as GameObject;
            }
            else
            {
                Debug.LogError($"资源未找到: {path}");
            }

            return asset;
        }

        /// <summary>  
        /// 清理缓存中的资源  
        /// </summary>  
        public void ClearCache()
        {
            assetCache.Clear();
        }
    }
}