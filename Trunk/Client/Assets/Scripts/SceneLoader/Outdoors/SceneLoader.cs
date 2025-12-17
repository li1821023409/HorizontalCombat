using System.Collections;
using System.Collections.Generic;
using UIFrame;
using UnityEngine;
using UnityEngine.SceneManagement;
using WNEngine;
using WNGameBase;

namespace WNGameBase
{
    /// <summary>
    /// 场景加载器，用于管理场景的加载和卸载
    /// </summary>
    public class SceneLoader : UnitySingleton<SceneLoader>
    {
        private GameInfo GameInfo;
        private GameBuilder GameBuilder;
        private string m_CurrentMapScene = string.Empty;
        public string CurrentMapScene
        {
            get { return m_CurrentMapScene; }
        }

        void Awake()
        {
            GameBuilder = GameBuilder.Instance;
            // 确保不会根据场景切换而销毁
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// 游戏基本信息构建
        /// </summary>
        public GameInfo BuilderGameInfo()
        {
            // TODO：切换场景时重新加载UI层级感觉不是很有必要，UI层级可以考虑常驻，可以用多场景叠加的方式实现地图切换
            // 后续觉得，这里用一个PersistentScene（持久场景）作为游戏逻辑的主要场景，Map场景作为加载场景，地图的切换由异步加载Map场景来实现
            //if (GameInfo != null)
            //{
            //    Reset();
            //}
            GameInfo = GameInfo.Instance;
            return GameInfo;
        }

        /// <summary>
        /// 开始游戏，加载常驻场景
        /// </summary>
        public void LoadPersistentScene(string mapScene)
        {
            // 先跳转到加载页面，加载常驻场景（也可以直接加载常驻场景并跳转，但是感觉这样好看一点）
            SceneManager.LoadScene("Loading");
            StartCoroutine(AsyncLoadPersistentScene());
            BuildMapScene(mapScene);
        }

        IEnumerator AsyncLoadPersistentScene()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(GameBuilder.PersistentSceneName);

            while (!operation.isDone)
            {
                yield return null;
            }

            Scene loadedScene = SceneManager.GetSceneByName(GameBuilder.PersistentSceneName);
            if (loadedScene.isLoaded)
            {
                SceneManager.SetActiveScene(loadedScene);
                GameBuilder.IsStartGame = true;
                GameBuilder.BuildGameScene();
                Debug.Log($"{GameBuilder.PersistentSceneName} 场景已激活!");
            }
            else
            {
                Debug.LogError($"{GameBuilder.PersistentSceneName} 加载失败或未能正确获取场景!");
            }
        }

        public void BuildMapScene(string mapScene)
        {
            // 当前地图和跳转地图一致则不跳转
            if (m_CurrentMapScene == mapScene)
            {
                Debug.LogError("SceneLoader.BuildMapScene m_CurrentMapScene == mapScene :" + CurrentMapScene);
                return;
            }
            StartCoroutine(BuildScene(mapScene));
        }

        IEnumerator BuildScene(string mapScene)
        {
            while (!GameBuilder.IsStartGame)
            {
                // Map场景构建的前提是游戏已经开始且PersistentScene构建完成
                yield return null;
            }

            if (mapScene == string.Empty)
                yield break;

            // TODO：感觉可以考虑把所有场景都加载出来，然后切换需要的场景？？? 以后再看吧

            // 加载新的map场景
            AsyncOperation loadSceen = SceneManager.LoadSceneAsync(mapScene, LoadSceneMode.Additive);
            while (!loadSceen.isDone)
            {
                yield return null;
            }
            yield return null;

            // 这里做一些加载完成后的操作处理
            Scene currentMapScene = SceneManager.GetSceneByName(mapScene);
            if (currentMapScene != null && currentMapScene.isLoaded)
            {
                SceneManager.SetActiveScene(currentMapScene);
                Debug.Log($"{mapScene} 地图场景已激活!");
                // Map场景为活跃场景后，搜索一下场景中的TilemapGrid并赋值

                // 获取当前激活场景
                Scene currentScene = SceneManager.GetActiveScene();
                // 打印场景的名字
                Debug.Log("Current active scene name: " + currentScene.name);

                SwitchSceneLoadData(currentScene);
                // 可能数据较大，建议等待一段时间
                yield return new WaitForSeconds(1f);
            }
            else
            {
                Debug.LogError("SceneLoader.BuildScene MapScene NotLoaded : " + mapScene);
                yield break;
            }

            // 新map场景加载后卸载旧的场景
            if (m_CurrentMapScene != string.Empty)
            {
                AsyncOperation unLoadSceen = SceneManager.UnloadSceneAsync(m_CurrentMapScene);
                while (!unLoadSceen.isDone)
                {
                    yield return null;
                }
            }

            // 加载完成
            m_CurrentMapScene = mapScene;
        }

        /// <summary>
        /// 用于切换场景时加载新场景相关数据
        /// </summary>
        public void SwitchSceneLoadData(Scene currentScene)
        {
            GameObject[] rootObj = currentScene.GetRootGameObjects();

            foreach (GameObject go in rootObj)
            {
                if (go.tag == StaticTag.TilemapGrid)
                {
                    GameBuilder.TilemapGrid = go.GetComponent<TilemapGrid>();
                    GameInfo.MovePawnToCurrentMap();
                    continue;
                }
                else if (go.tag == StaticTag.PlayerCamera)
                {
                    GameInfo.CameraManager.PlayerCamera = go.GetComponent<Camera>();
                }
            }

            // 玩家移到新场景且获取场景虚拟相机后，设置一下虚拟相机跟随
            GameInfo.CameraManager.VirtualCameraFollow();
        }

        // 移动Obj到目标场景的对应位置
        public void MoveGameObjectToScene(GameObject Obj, string assteId)
        {
            if (Obj == null) 
                return;

            // 检测Obj的应该在那个层级位置
            GameBuilder.SetObjParent(Obj, assteId);
        }
    }
}
