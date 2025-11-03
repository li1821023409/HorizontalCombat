// Create by DongShengLi At 2024/6/21
using System.Collections.Generic;

namespace UIFrame
{
    /// <summary>
    /// UI框架的事件管理器
    /// </summary>
    public class UIEventManager : UnitySingleton<UIEventManager>
    {
        public delegate void EventHandler(Param param);
        private Dictionary<string, EventHandler> dic = new Dictionary<string, EventHandler>();

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void AddListener(UIEvent eventType, EventHandler callback, string subkey = "")
        {
            var key = $"Event{eventType}_{subkey}";
            if (this.dic.ContainsKey(key))
            {
                this.dic[key] += callback;
            }
            else
            {
                this.dic.Add(key, callback);
            }
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void RemoveListener(UIEvent eventType, EventHandler callback, string subkey = "")
        {
            var key = $"Event{eventType}_{subkey}";
            if (!this.dic.ContainsKey(key))
            {
                return;
            }
            this.dic[key] -= callback;
            if (this.dic[key] == null)
            {
                this.dic.Remove(key);
            }
        }

        /// <summary>
        /// 触发事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="key"></param>
        /// <param name="param"></param>
        public void Emit(UIEvent eventType, string subkey = "", Param param = null)
        {
            var key = $"Event{eventType}_{subkey}";
            if (!this.dic.ContainsKey(key))
            {
                return;
            }
            this.dic[key](param);
        }
    }
}
