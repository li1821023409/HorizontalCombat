// Create by DongShengLi At 2024/6/21
using UnityEngine.UI;
using UnityEngine.Events;

namespace UIFrame
{
    public static class ButtonExtension
    {
        /// <summary>
        /// 添加按钮点击监听，封装了事件系统
        /// </summary>
        /// <param name="button"></param>
        public static void AddClickListener(this Button button, UnityAction callback, string subKey = null)
        {
            if (button == null) return;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                callback?.Invoke();
                // 事件通知，按钮点击了
                if (!string.IsNullOrEmpty(subKey))
                {
                    UIEventManager.Instance.Emit(UIEvent.BUTTON_CLICK, subKey);
                }
                UIEventManager.Instance.Emit(UIEvent.BUTTON_CLICK);
            });
        }

        /// <summary>
        /// 移除按钮点击监听，RemoveAllListeners
        /// </summary>
        public static void RemoveClickListener(this Button button, UnityAction callback = null)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
