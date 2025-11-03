using System.Collections;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WNGameBase
{
    public class Loading : MonoBehaviour
    {
        public Slider LoadSlider;
        public TextMeshProUGUI LoadText;

        void Start()
        {
            LoadNextLevel();
        }

        /// <summary>
        /// 加载下一个场景
        /// </summary>
        public void LoadNextLevel()
        {
            StartCoroutine(LoadLevel());
        }

        IEnumerator LoadLevel()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync("Outdoors");

            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                if (LoadText != null)
                    LoadText.text = Mathf.FloorToInt(operation.progress * 100) + " % 100";
                if (LoadSlider != null)
                    LoadSlider.value = operation.progress;

                yield return null;
            }
        }
    }
}