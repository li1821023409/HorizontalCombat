using System.Collections;
using System.Collections.Generic;
using UIFrame;
using UnityEngine;
using UnityEngine.SceneManagement;
using WNEngine;

namespace WNGameBase
{
    /// <summary>
    /// 场景加载器，用于管理场景的加载和卸载
    /// </summary>
    public class SceneLoader : UnitySingleton<SceneLoader>
    {
        private WNGame WNGame;
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
        public WNGame BuilderWNGame()
        {
            // TODO：切换场景时重新加载UI层级感觉不是很有必要，UI层级可以考虑常驻，可以用多场景叠加的方式实现地图切换
            // 后续修改：这里用一个PersistentScene（持久场景）作为游戏逻辑的主要场景，Map场景作为加载场景，地图的切换由异步加载Map场景来实现
            //if (GameInfo != null)
            //{
            //    Reset();
            //}
            WNGame = WNGame.Instance;
            return WNGame;
        }

        /// <summary>
        /// 开始游戏，加载常驻场景
        /// </summary>
        /// <param name="skipLoadingScene">是否跳过Loading场景（已在Loading场景时传true）</param>
        public void LoadPersistentScene(string mapScene, bool skipLoadingScene = false)
        {
            if (!skipLoadingScene)
            {
                SceneManager.LoadScene("Loading");
            }
            StartCoroutine(AsyncLoadPersistentScene());
            BuildMapScene(mapScene);
        }

        IEnumerator AsyncLoadPersistentScene()
        {
            // ── 阶段1：异步加载 PersistentScene ──
            AsyncOperation operation = SceneManager.LoadSceneAsync(GameBuilder.PersistentSceneName);
            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                ReportLoadProgress(0.7f + 0.2f * (operation.progress / 0.9f));
                yield return null;
            }

            // 允许场景激活
            operation.allowSceneActivation = true;
            while (!operation.isDone)
                yield return null;

            Scene persistentScene = SceneManager.GetSceneByName(GameBuilder.PersistentSceneName);
            if (!persistentScene.isLoaded)
            {
                Debug.LogError($"{GameBuilder.PersistentSceneName} 加载失败或未能正确获取场景!");
                yield break;
            }

            // 激活 PersistentScene
            SceneManager.SetActiveScene(persistentScene);
            ReportLoadProgress(0.95f);
            Debug.Log($"{GameBuilder.PersistentSceneName} 场景已激活!");

            // ── 阶段2：在 PersistentScene 中创建 Pool 层级节点 ──
            GameBuilder.InitPoolHierarchyInScene(persistentScene);
            // ── 阶段3：读取CSV配置并创建对象池（池对象归入刚创建的节点） ──
            GameBuilder.LoadAndSetupPools();

            // ── 阶段4：启动游戏逻辑 ──
            GameBuilder.IsStartGame = true;
            GameBuilder.BuildGameScene();
            ReportLoadProgress(1f);
        }

        /// <summary>
        /// 向当前场景中的Loading组件汇报进度
        /// </summary>
        private void ReportLoadProgress(float progress)
        {
            Loading loading = FindObjectOfType<Loading>();
            if (loading != null)
            {
                loading.SetLoadingData(Mathf.Clamp01(progress));
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
                if (go.tag == StaticTag.TILEMAP_GRID)
                {
                    GameBuilder.TilemapGrid = go.GetComponent<TilemapGrid>();
                    WNGame.MovePawnToCurrentMap();
                    continue;
                }
                else if (go.tag == StaticTag.PLAYER_CAMERA)
                {
                    WNGame.CameraManager.PlayerCamera = go.GetComponent<Camera>();
                }
            }

            // 玩家移到新场景且获取场景虚拟相机后，设置一下虚拟相机跟随
            WNGame.CameraManager.VirtualCameraFollow();
        }

        // 移动Obj到目标场景的对应位置
        public void MoveGameObjectToScene(GameObject Obj, string assteId)
        {
            if (Obj == null) 
                return;

            // 检测Obj的应该在那个层级位置
            GameBuilder.SetObjParent(Obj, assteId);
        }
        // msj + 生成战斗？
        // 核心玩法：挖矿、种田、科技、战斗、mjs培养?
    }
}
