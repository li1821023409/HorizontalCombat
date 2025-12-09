using System.Collections;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WNEngine;
using static UnityEngine.Rendering.HDROutputUtils;

namespace WNGameBase
{
    /// <summary>
    /// 用于进入游戏时候的首次加载页面，后续的异步加载由黑屏页面代替
    /// </summary>
    public class Loading : MonoBehaviour
    {
        public Slider LoadSlider;
        public TextMeshProUGUI LoadText;

        public void SetLoadingData(float value)
        {
            if (LoadText != null)
                LoadText.text = Mathf.FloorToInt(value / 0.9f * 100) + " % 100";
            if (LoadSlider != null)
                LoadSlider.value = value;
        }
    }
}