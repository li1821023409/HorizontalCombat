using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WNEngine;

namespace WNGameBase
{
    /// <summary>
    /// 用于进入游戏时候的加载页面
    /// 负责展示预加载 + 异步场景加载的整体进度
    /// </summary>
    public class Loading : MonoBehaviour
    {
        public Slider LoadSlider;
        public TextMeshProUGUI LoadText;

        /// <summary>
        /// 初始化加载页面，订阅GameBuilder的预加载进度
        /// </summary>
        private void Start()
        {
            // 订阅预加载进度（如果GameBuilder已存在）
            if (GameBuilder.HasInstance)
            {
                GameBuilder.Instance.OnPreloadProgress += OnPreloadProgress;
                GameBuilder.Instance.OnPreloadCompleted += OnPreloadCompleted;
            }
        }

        private void OnDestroy()
        {
            if (GameBuilder.HasInstance)
            {
                GameBuilder.Instance.OnPreloadProgress -= OnPreloadProgress;
                GameBuilder.Instance.OnPreloadCompleted -= OnPreloadCompleted;
            }
        }

        private void OnPreloadProgress(float progress)
        {
            SetLoadingData(progress);
        }

        private void OnPreloadCompleted()
        {
            // 预加载完成，显示进度满格
            SetLoadingData(1f);
        }

        /// <summary>
        /// 设置加载进度显示（0 ~ 1）
        /// 内部映射到UI的0-100显示
        /// </summary>
        public void SetLoadingData(float value)
        {
            float displayValue = Mathf.Clamp01(value);
            if (LoadSlider != null)
                LoadSlider.value = displayValue;
            if (LoadText != null)
                LoadText.text = Mathf.FloorToInt(displayValue * 100) + " %";
        }
    }
}