// Create by DongShengLi At 2024/6/21
using UnityEngine.UI;
using UnityEngine.Events;

namespace UIFrame
{
    public static class SliderExtension
    {
        /// <summary>
        /// 添加slider滑动监听，封装了事件系统
        /// </summary>
        /// <param name="slider"></param>
        public static void OnValueChangedLisener(this Slider slider, UnityAction<float> callback, string subKey = null)
        {
            if (slider == null) return;
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener((value) =>
            {
                callback?.Invoke(value);
                // 事件通知，slider数值变化
                //if (!string.IsNullOrEmpty(subKey))
                //{
                //    UIEventManager.Instance.Emit(UIEvent.SLIDE_VALUE_CHANGE, subKey, value.ToString());
                //}
                //UIEventManager.Instance.Emit(UIEvent.SLIDE_VALUE_CHANGE, "", value.ToString());
            });
        }

        /// <summary>
        /// 移除滑动监听，RemoveAllListeners
        /// </summary>
        public static void RemoveOnValueChangedLisener(this Slider slider, UnityAction callback = null)
        {
            slider.onValueChanged.RemoveAllListeners();
        }
    }
}
