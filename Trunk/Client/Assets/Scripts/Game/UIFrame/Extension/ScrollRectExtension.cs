// Create by DongShengLi At 2024/7/6
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIFrame
{
    public static class ScrollRectExtension
    {

        /// <summary>
        /// 添加滚动监听，到达指定进度时触发一次 
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="callback"></param>
        /// <param name="isVertical"></param>
        /// <param name="progress">滚动条进度，1-100</param>
        /// <param name="subKey"></param>
        public static void AddScrollListener(this ScrollRect scrollRect, UnityAction callback, bool isVertical, string subKey = null, int progress = 100)
        {
            if (scrollRect == null) return;
            scrollRect.onValueChanged.RemoveAllListeners();
            scrollRect.onValueChanged.AddListener((Vector2 position) =>
            {
                float pos = 0;
                if (isVertical)
                {
                    pos = scrollRect.verticalNormalizedPosition;
                } else
                {
                    pos = scrollRect.horizontalNormalizedPosition;
                }

                if (Mathf.RoundToInt(pos * 100) >= progress)
                {
                    // 只触发一次
                    scrollRect.onValueChanged.RemoveAllListeners();

                    callback?.Invoke();
                    // 事件通知，按钮点击了
                    if (!string.IsNullOrEmpty(subKey))
                    {
                        UIEventManager.Instance.Emit(UIEvent.SCROLL_VIEW_SCROLL, subKey);
                    }
                }
            });
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="scrollRect"></param>
        public static void RemoveAllScrollListener(this ScrollRect scrollRect)
        {
            if (scrollRect == null) return;
            scrollRect.onValueChanged.RemoveAllListeners();
        }
    }
}
