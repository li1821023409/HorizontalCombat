using System.Collections.Generic;
using UnityEngine;

namespace UIFrame
{
    public class ResMgr : UnitySingleton<ResMgr>
    {
        private Dictionary<string, GameObject> assetCache = new Dictionary<string, GameObject>();

        /// <summary>  
        /// �ӻ����л�ȡ��Դ����������в����������Դ�ļ��м��ز�����  
        /// </summary>  
        /// <param name="path">��Դ·��</param>  
        /// <returns>GameObject ��Դ</returns>  
        public T GetAssetCache<T>(string path) where T : UnityEngine.Object
        {
            // ��黺�����Ƿ����  
            if (assetCache.TryGetValue(path, out var cachedAsset))
            {
                return cachedAsset as T;
            }

            // ����Դ�ļ��м���  
            T asset = Resources.Load<T>(path);
            if (asset != null)
            {
                // ��ӵ�����  
                assetCache[path] = asset as GameObject;
            }
            else
            {
                Debug.LogError($"��Դδ�ҵ�: {path}");
            }

            return asset;
        }

        /// <summary>  
        /// ��������е���Դ  
        /// </summary>  
        public void ClearCache()
        {
            assetCache.Clear();
        }
    }
}