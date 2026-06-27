using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using WNEngine;

namespace WNGameBase
{
    /// <summary>
    /// 游戏入口：加载 Loading 场景 → 委托 SceneLoader 执行完整加载管线
    /// 完整流程：Loading展示 → 加载PersistentScene → 在其中创建Pool节点
    ///          → 预加载CSV+池对象 → 加载MapScene
    /// </summary>
    public class Login : MonoBehaviour
    {
        public GameBuilder m_GameBuilder;

        private void Start()
        {
            m_GameBuilder = GameBuilder.Instance;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(LoadWithPreload());
        }

        private IEnumerator LoadWithPreload()
        {
            // 1. 叠加加载Loading场景
            AsyncOperation loadLoading = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
            while (!loadLoading.isDone)
                yield return null;

            // 2. 激活Loading场景
            Scene loadingScene = SceneManager.GetSceneByName("Loading");
            if (loadingScene.isLoaded)
            {
                SceneManager.SetActiveScene(loadingScene);
            }

            // 3. 委托SceneLoader执行完整加载管线：
            //    PersistentScene加载 → Pool层级创建 → CSV+池预加载 → 游戏启动
            m_GameBuilder.SceneLoader.LoadPersistentScene(m_GameBuilder.MapSceneName, true);
        }
    }
}
