// Create by DongShengLi At 2024/6/21
using System.Collections.Generic;
using UIFrame;

namespace WNGameBase
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class UIEventManager : UnitySingleton<UIEventManager>
    {
        #region UI的事件框架
        public delegate void UIEventHandler(Param param);
        private Dictionary<string, UIEventHandler> uiEventDic = new Dictionary<string, UIEventHandler>();

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void AddUIEventListener(UIEvent eventType, UIEventHandler callback, string subkey = "")
        {
            var key = $"Event{eventType}_{subkey}";
            if (this.uiEventDic.ContainsKey(key))
            {
                this.uiEventDic[key] += callback;
            }
            else
            {
                this.uiEventDic.Add(key, callback);
            }
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void RemoveUIEventListener(UIEvent eventType, UIEventHandler callback, string subkey = "")
        {
            var key = $"Event{eventType}_{subkey}";
            if (!this.uiEventDic.ContainsKey(key))
            {
                return;
            }
            this.uiEventDic[key] -= callback;
            if (this.uiEventDic[key] == null)
            {
                this.uiEventDic.Remove(key);
            }
        }

        /// <summary>
        /// 触发事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="key"></param>
        /// <param name="param"></param>
        public void UIEventEmit(UIEvent eventType, string subkey = "", Param param = null)
        {
            var key = $"Event{eventType}_{subkey}";
            if (!this.uiEventDic.ContainsKey(key))
            {
                return;
            }
            this.uiEventDic[key](param);
        }
        #endregion

        #region GamePlay的事件框架
        public delegate void GamePlayEventHandler(Param param);
        private Dictionary<string, GamePlayEventHandler> gamePlayEventDic = new Dictionary<string, GamePlayEventHandler>();

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void AddGamePlayEventListener(GamePlayEvent eventType, GamePlayEventHandler callback, string subkey = "")
        {
            var key = $"Event{eventType}_{subkey}";
            if (this.gamePlayEventDic.ContainsKey(key))
            {
                this.gamePlayEventDic[key] += callback;
            }
            else
            {
                this.gamePlayEventDic.Add(key, callback);
            }
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void RemoveGamePlayEventListener(GamePlayEvent eventType, GamePlayEventHandler callback, string subkey = "")
        {
            var key = $"Event{eventType}_{subkey}";
            if (!this.gamePlayEventDic.ContainsKey(key))
            {
                return;
            }
            this.gamePlayEventDic[key] -= callback;
            if (this.gamePlayEventDic[key] == null)
            {
                this.gamePlayEventDic.Remove(key);
            }
        }

        /// <summary>
        /// 触发事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="key"></param>
        /// <param name="param"></param>
        public void GamePlayEventEmit(GamePlayEvent eventType, string subkey = "", Param param = null)
        {
            var key = $"Event{eventType}_{subkey}";
            if (!this.gamePlayEventDic.ContainsKey(key))
            {
                return;
            }
            this.gamePlayEventDic[key](param);
        }
        #endregion
    }
}
