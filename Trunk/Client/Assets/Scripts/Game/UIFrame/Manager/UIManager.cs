// Create by DongShengLi At  2024/4/16
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WNGameBase;

namespace UIFrame
{
    /// <summary>
    /// 整个UI框架的入口类，在场景上添加可使用
    /// </summary>
    public class UIManager : UnitySingleton<UIManager>
    {
        #region 基础内容，创建节点，初始化

        /// <summary>
        /// Canvas，UI根节点
        /// </summary>
        private GameObject baseCanvas;

        /// <summary>
        /// 界面层级节点，Base
        /// </summary>
        private Transform CanvasLevelBase;
        /// <summary>
        /// 界面层级节点，Main
        /// </summary>
        private Transform CanvasLevelMain;
        /// <summary>
        /// 界面层级节点，Toast
        /// </summary>
        private Transform CanvasLevelToast;
        /// <summary>
        /// 界面层级节点，Loading
        /// </summary>
        private Transform CanvasLevelLoading;

        /// <summary>
        /// panel自动销毁的时间间隔
        /// </summary>
        private WaitForSeconds panelAutoCloseWait = new WaitForSeconds(UIConst.VIEW_DESTORY_INTERVAL);

        private void Awake()
        {
            // 初始化，所有UI的根节点
            baseCanvas = GameObject.Find("Canvas");
            if (baseCanvas == null)
            {
                throw new UIFrameException("Canvas不存在");
            }

            // 初始化，调整CanvasScaler适配比例
            var scaler = baseCanvas.GetComponent<CanvasScaler>();
            scaler.matchWidthOrHeight = CheckCanvasScalerMatch();
            // 初始化，是否是窄屏
            isAdaptWidth = IsPhone();

            // 初始化，判断层级的存在，如果没有，要求加上
            if (LayerMask.NameToLayer("UIHide") < 0)
            {
                throw new UIFrameException("UIHide，这个层级是必须的，请在layer中添加并在摄像机中过滤掉，用于UI隐藏");
            }
            // 初始化，判断层级的存在，如果没有，要求加上
            if (LayerMask.NameToLayer("UIShow") < 0)
            {
                throw new UIFrameException("UIShow，这个层级是必须的，请在layer中添加，用于UI显示");
            }

            // 创建Canvas多个层级容器，对应层级的面板会放入对应层中
            CanvasLevelBase = CreateCanvasLevel(GetCanvasLevelName(ViewCanvasLevel.BASE), (int)ViewCanvasLevel.BASE);
            CanvasLevelMain = CreateCanvasLevel(GetCanvasLevelName(ViewCanvasLevel.MAIN), (int)ViewCanvasLevel.MAIN);
            CanvasLevelToast = CreateCanvasLevel(GetCanvasLevelName(ViewCanvasLevel.TOAST), (int)ViewCanvasLevel.TOAST);
            CanvasLevelLoading = CreateCanvasLevel(GetCanvasLevelName(ViewCanvasLevel.LOADING), (int)ViewCanvasLevel.LOADING);

            // 不销毁
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// 创建Canvas层级，对UI进行分层
        /// </summary>
        private Transform CreateCanvasLevel(string name, int level)
        {
            GameObject obj = new GameObject(name);
            if (baseCanvas != null)
            {
                obj.transform.SetParent(baseCanvas.transform);
                obj.transform.localScale = Vector3.one;
                // 增加RectTransform组件
                var rect = obj.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.anchoredPosition3D = new Vector3(0, 0, -level);
                rect.sizeDelta = Vector2.zero;
                // 增加Canvas组件
                var canvas = obj.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = level;
                // 增加GraphicRaycaster组件
                obj.AddComponent<GraphicRaycaster>();
                return obj.transform;
            }
            else
            {
                throw new UIFrameException("Canvas不存在");
            }
        }

        /// <summary>
        /// 获取指定层级对象名称
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetCanvasLevelName(ViewCanvasLevel level)
        {
            string name = null;
            switch (level)
            {
                case ViewCanvasLevel.BASE:
                    name = "BASE";
                    break;
                case ViewCanvasLevel.MAIN:
                    name = "MAIN";
                    break;
                case ViewCanvasLevel.TOAST:
                    name = "TOAST";
                    break;
                case ViewCanvasLevel.LOADING:
                    name = "LOADING";
                    break;
                default:
                    break;
            }
            return name;
        }

        /// <summary>
        /// 获取指定层的下一层
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private ViewCanvasLevel GetCanvasLevelUnder(ViewCanvasLevel level)
        {
            ViewCanvasLevel res = ViewCanvasLevel.NONE;
            switch (level)
            {
                case ViewCanvasLevel.BASE:
                    break;
                case ViewCanvasLevel.MAIN:
                    res = ViewCanvasLevel.BASE;
                    break;
                case ViewCanvasLevel.TOAST:
                    res = ViewCanvasLevel.MAIN;
                    break;
                case ViewCanvasLevel.LOADING:
                    res = ViewCanvasLevel.TOAST;
                    break;
            }
            return res;
        }

        #endregion

        #region Canvas适配相关内容

        /// <summary>
        /// 调整适配比例
        /// </summary>
        private float CheckCanvasScalerMatch()
        {
            //按平板来算 相对来说，横屏
            if (Screen.height > 1500f) { return 1f; }

            // float originalRatio = 1559f / 1112f; // 1280f/720f
            float originalRatio = 1.7f;
            float currentRatio = Screen.width * 1.0f / Screen.height;

            //按平板来算
            if (originalRatio >= currentRatio) { return 1f; }
            //按手机来算
            else
            {
                float phoneMatchValue = currentRatio - 1.8f;
                if (phoneMatchValue > 0.45f)
                {
                    phoneMatchValue = 0.45f;
                }
                else if (phoneMatchValue > 0.3f)
                {
                    phoneMatchValue = 0.3f;
                }
                else if (phoneMatchValue < 0f)
                {
                    phoneMatchValue = 0f;
                }
                return phoneMatchValue;
            }
        }

        /// <summary>
        /// 是否是手机
        /// </summary>
        /// <returns></returns>
        private bool IsPhone()
        {
            bool isPhone = true;
#if UNITY_IPHONE && !UNITY_EDITOR
			string deviceInfo = SystemInfo.deviceModel.ToString();
			isPhone = deviceInfo.Contains("iPhone");
#else
            float physicscreen = 1.0f * Screen.width / Screen.height;
            isPhone = Mathf.Round(physicscreen * 10) > 15; //(the ratio 4:3 = 1.33; 16:9 = 1.777;)
#endif
            return isPhone;
        }

        #endregion

        #region 界面列表相关内容

        /// <summary>
        /// 当前显示界面列表
        /// </summary>
        private List<BasePanel> showPanelList = new List<BasePanel>();
        /// <summary>
        /// 当前隐藏界面列表
        /// </summary>
        private List<BasePanel> hidePanelList = new List<BasePanel>();
        /// <summary>
        /// 当前关闭界面列表
        /// </summary>
        private List<BasePanel> closePanelList = new List<BasePanel>();

        /// <summary>
        /// 视图列表，Base
        /// </summary>
        private List<BasePanel> panelBaseList = new List<BasePanel>();
        /// <summary>
        /// 视图列表，Main
        /// </summary>
        private List<BasePanel> panelMainList = new List<BasePanel>();
        /// <summary>
        /// 视图列表，Toast
        /// </summary>
        private List<BasePanel> panelToastList = new List<BasePanel>();
        /// <summary>
        /// 视图列表，Loading
        /// </summary>
        private List<BasePanel> panelLoadingList = new List<BasePanel>();

        /// <summary>
        /// 获取指定层级列表
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private List<BasePanel> GetListByLevel(ViewCanvasLevel level)
        {
            List<BasePanel> list = null;
            switch (level)
            {
                case ViewCanvasLevel.BASE:
                    list = panelBaseList;
                    break;
                case ViewCanvasLevel.MAIN:
                    list = panelMainList;
                    break;
                case ViewCanvasLevel.TOAST:
                    list = panelToastList;
                    break;
                case ViewCanvasLevel.LOADING:
                    list = panelLoadingList;
                    break;
            }
            return list;
        }

        /// <summary>
        /// 将Panel加入层级列表
        /// </summary>
        /// <param name="basePanel"></param>
        private int AddPanelToLevelList(BasePanel basePanel)
        {
            var list = GetListByLevel(basePanel.canvasLevel);
            list.Add(basePanel);
            return list.Count;
        }

        /// <summary>
        /// 获取指定层级底部panel的索引
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private int GetTopPanelOrderIndex(ViewCanvasLevel level)
        {
            var list = GetListByLevel(level);
            if (list.Count > 0)
            {
                return list[list.Count - 1].orderIndex;
            } else
            {
                return 0;
            }
        }

        /// <summary>
        /// 将Panel移出层级列表列表
        /// </summary>
        /// <param name="basePanel"></param>
        private void RemovePanelFromList(BasePanel basePanel)
        {
            var list = GetListByLevel(basePanel.canvasLevel);
            list.Remove(basePanel);
        }

        /// <summary>
        /// 移出显示列表
        /// </summary>
        /// <param name="basePanel"></param>
        private void RemovePanelFromShowList(BasePanel basePanel)
        {
            if (basePanel != null && showPanelList.Contains(basePanel))
            {
                showPanelList.Remove(basePanel);
            }
        }

        /// <summary>
        /// 移除隐藏列表
        /// </summary>
        /// <param name="basePanel"></param>
        private void RemovePanelFromHideList(BasePanel basePanel)
        {
            if (basePanel != null && hidePanelList.Contains(basePanel))
            {
                hidePanelList.Remove(basePanel);
            }
        }

        /// <summary>
        /// 移出关闭列表
        /// </summary>
        /// <param name="basePanel"></param>
        private void RemovePanelFromCloseList(BasePanel basePanel)
        {
            if (basePanel != null && closePanelList.Contains(basePanel))
            {
                closePanelList.Remove(basePanel);
            }
        }

        #endregion

        #region Panel相关内容

        /// <summary>
        /// 创建Panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private BasePanel DoCreatePanel<T>(Param param = null, Transform parent = null) where T : BasePanel
        {
            // 创建脚本
            var basePanel = gameObject.AddComponent<T>();
            basePanel.PanelName = typeof(T).ToString();
            // 将参数存入
            basePanel.showParam = param;

            // -----------------------Panel生命周期---------------------
            // 脚本创建了，触发生命周期方法，OnCreate()
            basePanel.OnCreate();
            // -----------------------Panel生命周期---------------------

            // 判断当前屏幕情况，如果有配置手机屏预制体，按需加载
            string loadPath;
            if (isAdaptWidth)
            {
                loadPath = basePanel.resPathPhone ?? basePanel.resPath;
            }
            else
            {
                loadPath = basePanel.resPath;
            }
            var go = LoadPanelRes(loadPath);
            go.name = typeof(T).ToString();
            // 视图资源加载完毕
            return OnViewLoaded(basePanel, go, parent);
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="loadPath"></param>
        /// <returns></returns>
        private GameObject LoadPanelRes(string loadPath)
        {
            // 加载资源
            GameObject prefab = ResMgr.Instance.GetAssetCache<GameObject>(loadPath);
            return Instantiate(prefab);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator LoadPanelResSync<T>(string path, UnityAction<GameObject> callback)
        {
            // TODO 异步加载预制体
            yield return null;
        }

        /// <summary>
        /// 预制体加载完毕
        /// </summary>
        /// <param name="basePanel"></param>
        /// <param name="index"></param>
        /// <param name="go">创建的GameObject</param>
        private BasePanel OnViewLoaded<T>(T basePanel, GameObject go, Transform parent = null) where T : BasePanel
        {
            if (parent == null)
            {
                // 根据配置的层级，加载到指定节点下面
                switch (basePanel.canvasLevel)
                {
                    case ViewCanvasLevel.BASE:
                        go.transform.SetParent(CanvasLevelBase);
                        break;
                    case ViewCanvasLevel.MAIN:
                        go.transform.SetParent(CanvasLevelMain);
                        break;
                    case ViewCanvasLevel.TOAST:
                        go.transform.SetParent(CanvasLevelToast);
                        break;
                    case ViewCanvasLevel.LOADING:
                        go.transform.SetParent(CanvasLevelLoading);
                        break;
                }
                go.transform.localScale = Vector3.one;
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

                // 设置order，根据顶部索引，+1
                var index = GetTopPanelOrderIndex(basePanel.canvasLevel);
                basePanel.orderIndex = index + 1;
                // 设置z轴位置，偏移量根据厚度*索引计算
                var offset = basePanel.orderIndex * UIConst.VIEW_THICK;
                go.transform.localPosition = new Vector3(0, 0, -offset);
                // 添加Canvas组件
                var canvas = go.GetComponent<Canvas>();
                if (canvas == null) { canvas = go.AddComponent<Canvas>(); }
                canvas.overrideSorting = true;
                canvas.sortingOrder = (int)basePanel.canvasLevel + offset;

                // 加入层级列表
                AddPanelToLevelList(basePanel);
            }
            else
            {
                // 加载到指定的容器节点下面
                go.transform.SetParent(parent);
                go.transform.localScale = Vector3.one;
                var rect = go.GetComponent<RectTransform>();
                rect.anchoredPosition3D = Vector2.zero;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }

            // 增加GraphicRaycaster组件，用于接收射线
            if (go.GetComponent<GraphicRaycaster>() == null) { go.AddComponent<GraphicRaycaster>(); }
            // 增加CanvasGroup组件，只用于需要做动画的场景，不用于显示隐藏
            if (go.GetComponent<CanvasGroup>() == null) { go.AddComponent<CanvasGroup>(); }


            // 初始化视图
            if (basePanel.panleView == null) { basePanel.InitPanelView(go); }

            // -----------------------Panel生命周期---------------------
            // 视图gameobject创建了，触发生命周期方法，OnViewLoaded()
            basePanel.OnViewLoaded();
            // 全部初始化完毕，执行视图回调
            basePanel.viewLoadedEvent?.Invoke();
            // -----------------------Panel生命周期---------------------

            // 显示panel
            return DoShowPanel(basePanel);
        }

        /// <summary>
        /// 显示panel
        /// </summary>
        /// <param name="basePanel"></param>
        private BasePanel DoShowPanel<T>(T basePanel, bool isBeShow = false) where T : BasePanel
        {
            // 已经在显示列表里了，不需要处理
            if (showPanelList.Contains(basePanel)) return basePanel;
            basePanel.isShow = true;
            basePanel.isBeShow = isBeShow;

            // 优化处理，如果是全屏界面，把看不见的界面全部隐藏
            if (!isBeShow && basePanel.coverType == ViewCoverType.FULLSCREEN)
            {
                var list = GetListByLevel(basePanel.canvasLevel);
                var index = list.IndexOf(basePanel);
                if (index >= 0)
                {
                    BetterShowOrHidePanel(basePanel.canvasLevel, true, index - 1);
                }
            }

            // -----------------------Panel生命周期---------------------
            // 触发生命周期方法，OnBeforeShow()
            basePanel.OnBeforeShow();
            // -----------------------Panel生命周期---------------------

            // 显示
            ViewGameObjectShow(basePanel.viewGameObject, true);

            // -----------------------Panel生命周期---------------------
            // 触发生命周期方法，OnShow()
            basePanel.OnShow();
            // -----------------------Panel生命周期---------------------

            // 加入显示列表
            showPanelList.Add(basePanel);
            RemovePanelFromHideList(basePanel);
            RemovePanelFromCloseList(basePanel);

            // 发送事件通知
            UIEventManager.Instance.UIEventEmit(UIEvent.PANEL_SHOW, basePanel.PanelName);
            UIEventManager.Instance.UIEventEmit(UIEvent.PANEL_SHOW);

            return basePanel;
        }

        /// <summary>
        /// 隐藏panel
        /// </summary>
        /// <param name="basePanel"></param>
        private void DoHidePanel<T>(T basePanel, bool isBeHide = false) where T : BasePanel
        {
            // 已经在隐藏列表里了，不需要处理
            if (hidePanelList.Contains(basePanel)) return;
            basePanel.isShow = false;
            basePanel.isBeHide = isBeHide;

            // -----------------------Panel生命周期---------------------
            // 触发生命周期方法，OnBeforeHide()
            var waitTime = basePanel.OnBeforeHide();
            // -----------------------Panel生命周期---------------------

            // 如果设置了延迟，晚点再隐藏GameObject
            if (waitTime > 0)
            {
                StartCoroutine(IHidePanelObj(basePanel.viewGameObject, waitTime));
            } else
            {
                // 立即关闭
                ViewGameObjectShow(basePanel.viewGameObject, false);
            }

            // -----------------------Panel生命周期---------------------
            // 触发生命周期方法，OnCreate()
            basePanel.OnHide();
            // -----------------------Panel生命周期---------------------

            // 加入隐藏列表
            hidePanelList.Add(basePanel);
            RemovePanelFromShowList(basePanel);
            RemovePanelFromCloseList(basePanel);

            // 优化处理，如果是全屏界面，把底下隐藏的界面全部重新显示
            if (!isBeHide && basePanel.coverType == ViewCoverType.FULLSCREEN)
            {
                var list = GetListByLevel(basePanel.canvasLevel);
                var index = list.IndexOf(basePanel);
                if (index >= 0)
                {
                    BetterShowOrHidePanel(basePanel.canvasLevel, false, index - 1);
                }
            }

            // 发送事件通知
            UIEventManager.Instance.UIEventEmit(UIEvent.PANEL_HIDE, basePanel.PanelName);
            UIEventManager.Instance.UIEventEmit(UIEvent.PANEL_HIDE);
        }

        /// <summary>
        /// 隐藏GameObject的协程
        /// </summary>
        /// <param name="panelObj"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        private IEnumerator IHidePanelObj(GameObject panelObj, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            ViewGameObjectShow(panelObj, false);
        }

        /// <summary>
        /// 关闭Panel，销毁视图
        /// </summary>
        /// <param name="basePanel"></param>
        private void DoClosePanel<T>(T basePanel) where T : BasePanel
        {
            // 移出列表
            RemovePanelFromShowList(basePanel);
            RemovePanelFromHideList(basePanel);
            // 移出层级列表
            RemovePanelFromList(basePanel);

            // -----------------------Panel生命周期---------------------
            // 脚本创建了，触发生命周期方法，OnClose()
            basePanel.OnClose();
            // 全部初始化完毕，清空视图回调
            basePanel.viewLoadedEvent.RemoveAllListeners();
            // -----------------------Panel生命周期---------------------

            // 加入关闭列表
            if (!closePanelList.Contains(basePanel))
            {
                closePanelList.Add(basePanel);
            }

            // 发送事件通知
            UIEventManager.Instance.UIEventEmit(UIEvent.PANEL_CLOSE, basePanel.PanelName);
            UIEventManager.Instance.UIEventEmit(UIEvent.PANEL_CLOSE);

            // 根据销毁方式进行销毁
            if (basePanel.hasParent)
            {
                // 跟随父界面
                // TODO 马上销毁
                DoDestoryView(basePanel);
            } else
            {
                switch (basePanel.destoryType)
                {
                    case PanelDestoryType.AUTO:
                        // 自动销毁，启动协程，达到指定时长后销毁
                        if (basePanel.panelCloseCorotine != null)
                        {
                            StopCoroutine(basePanel.panelCloseCorotine);
                            basePanel.panelCloseCorotine = null;
                        }
                        basePanel.panelCloseCorotine = StartCoroutine(PanelAutoDestoryCoroutine(basePanel));
                        break;
                    case PanelDestoryType.IMMEDIATELY:
                        // 马上销毁
                        DoDestoryView(basePanel);
                        break;
                    case PanelDestoryType.NEVER:
                        // 从不销毁，不做处理
                        break;
                }
            }
        }

        /// <summary>
        /// 销毁关联的GameObject
        /// </summary>
        /// <param name="basePanel"></param>
        private void DoDestoryView<T>(T basePanel) where T : BasePanel
        {
            // 移出关闭列表
            closePanelList.Remove(basePanel);

            // 销毁视图
            basePanel.panleView?.ClearView();
            if (basePanel.viewGameObject != null)
            {
                GameObject.Destroy(basePanel.viewGameObject);
                basePanel.viewGameObject = null;
            }

            // -----------------------Panel生命周期---------------------
            // 脚本创建了，触发生命周期方法，OnViewDestroy()
            basePanel.OnViewDestroy();
            // -----------------------Panel生命周期---------------------

            // 移除组件
            Destroy(basePanel);
        }

        /// <summary>
        /// Panel自动关闭的协程，时间为常量
        /// </summary>
        /// <param name="basePanel"></param>
        /// <returns></returns>
        private IEnumerator PanelAutoDestoryCoroutine(BasePanel basePanel)
        {
            yield return panelAutoCloseWait;
            DoDestoryView(basePanel);
        }

        /// <summary>
        /// 对panel显示/隐藏做优化处理，当全屏显示的界面显示时，把底层的界面能隐藏的隐藏，能显示的显示
        /// </summary>
        /// <param name="level"></param>
        /// <param name="isShow">是由于界面显示触发的，还是隐藏触发的</param>
        /// <param name="startIndex">开始检测的位置，应该为当前界面的下一个位置，默认为-1</param>
        /// <returns></returns>
        private bool BetterShowOrHidePanel(ViewCanvasLevel level, bool isShow, int startIndex = -2)
        {
            if (level == ViewCanvasLevel.NONE) { return false; }
            // 拿出当前层的列表
            var curLvelList = GetListByLevel(level);
            // 默认列表末尾开始
            if (startIndex < -1) { startIndex = curLvelList.Count - 1; }
            if (startIndex >= 0) {
                // 从后往前遍历
                for (int i = startIndex; i >= 0; i--)
                {
                    var panel = curLvelList[i];
                    // 隐藏/显示底下的界面
                    if (isShow) { DoHidePanel(panel, true); }
                    else { DoShowPanel(panel, true); }
                    // 如果当前是全屏了，不再继续往下找了
                    if (panel.coverType == ViewCoverType.FULLSCREEN) { return true; }
                }
            }

            // 本层没找到，继续往下找
            var underLevel = GetCanvasLevelUnder(level);
            return BetterShowOrHidePanel(underLevel, isShow);
        }

        /// <summary>
        /// 显示界面，panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param">可选参数，使用ParamBuilder创建，例：new ParamBuilder().AppendInt(1).Build()</param>
        private BasePanel ShowPanel<T>(Param param, Transform parent) where T : BasePanel
        {
            // 去隐藏列表找，弹出上面全部界面
            var hidePanel = hidePanelList.FindLast((panel) => panel is T);
            if (hidePanel != null)
            {
                //  如果有关闭协程还在运行，停止
                if (hidePanel.panelCloseCorotine != null)
                {
                    StopCoroutine(hidePanel.panelCloseCorotine);
                    hidePanel.panelCloseCorotine = null;
                }
                // 将参数存入
                hidePanel.showParam = param;

                // 把上层的界面全部弹出, 只处理Mian层
                if (parent == null && hidePanel.canvasLevel == ViewCanvasLevel.MAIN)
                {
                    var list = GetListByLevel(ViewCanvasLevel.MAIN);
                    var startIndex = list.IndexOf(hidePanel);
                    for (int i = list.Count - 1; i > startIndex; i--)
                    {
                        // 先隐藏
                        DoHidePanel(list[i], true);
                        // 关闭
                        DoClosePanel(list[i]);
                    }
                }

                // 重新显示
                return DoShowPanel(hidePanel);
            }

            // 去关闭列表找，回收再使用
            var closePanel = closePanelList.FindLast((panel) => panel is T);
            if (closePanel != null)
            {
                //  如果有关闭协程还在运行，停止
                if (closePanel.panelCloseCorotine != null)
                {
                    StopCoroutine(closePanel.panelCloseCorotine);
                    closePanel.panelCloseCorotine = null;
                }
                // 将参数存入
                closePanel.showParam = param;

                // -----------------------Panel生命周期---------------------
                // 脚本创建了，触发生命周期方法，OnCreate()
                closePanel.OnCreate();
                // -----------------------Panel生命周期---------------------

                // 重新显示
                return OnViewLoaded(closePanel, closePanel.viewGameObject, parent);
            }

            // 去显示列表找，如果存在，不做处理
            var showPanel = showPanelList.FindLast((panel) => panel is T);
            if (showPanel == null)
            {
                return DoCreatePanel<T>(param, parent);
            }
            return showPanel;
        }

        #endregion

        #region 公共方法，提供给外部使用
        /// <summary>
        /// 显示界面，panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param">可选参数，使用ParamBuilder创建，例：new ParamBuilder().AppendInt(1).Build()</param>
        public T ShowPanel<T>(Param param = null) where T : BasePanel
        {
            return ShowPanel<T>(param, null) as T;
        }

        /// <summary>
        /// 关闭指定界面，走协程，不会马上关闭
        /// </summary>
        /// <param name="basePanel"></param>
        public void ClosePanel<T>(T basePanel = null) where T : BasePanel
        {
            ClosePanelImmediately(basePanel);
            //if (basePanel == null)
            //{
            //    // 根据泛型去显示列表找
            //    var showPanel = GetShowPanel<T>();
            //    if (showPanel != null)
            //    {
            //        StartCoroutine(IClosPanel(showPanel));
            //    }
            //} else
            //{
            //    StartCoroutine(IClosPanel(basePanel));
            //}
        }

        /// <summary>
        /// 马上关闭指定界面，不走协程，OnBeforeHide返回时间强制为0
        /// </summary>
        /// <param name="basePanel"></param>
        public void ClosePanelImmediately<T>(T basePanel = null) where T : BasePanel
        {
            if (basePanel == null)
            {
                // 根据泛型去显示列表找
                var showPanel = GetShowPanel<T>();
                if (showPanel != null)
                {
                    // 先隐藏
                    DoHidePanel(showPanel);
                    // 关闭
                    DoClosePanel(showPanel);
                }
            }
            else
            {
                // 先隐藏
                DoHidePanel(basePanel);
                // 关闭
                DoClosePanel(basePanel);
            }
        }

        /// <summary>
        /// 显示子panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">传入容器节点</param>
        public void ShowSubPanel<T>(Transform parent, Param param = null) where T : BasePanel
        {
            ShowPanel<T>(param, parent);
        }

        /// <summary>
        /// 显示指定的子界面
        /// </summary>
        /// <param name="basePanel"></param>
        /// <param name="isBeShow"></param>
        public void ShowSubPanel(BasePanel basePanel, Param param = null, bool isBeShow = false)
        {
            // 去隐藏列表找，如果有则显示，没有则创建
            if (basePanel != null && hidePanelList.Contains(basePanel))
            {
                //  如果有关闭协程还在运行，停止
                if (basePanel.panelCloseCorotine != null)
                {
                    StopCoroutine(basePanel.panelCloseCorotine);
                    basePanel.panelCloseCorotine = null;
                }
                // 如果是主动显示的，需要将参数存入
                if (!isBeShow) { basePanel.showParam = param; }
                // 重新显示
                DoShowPanel(basePanel, isBeShow);
            }
        }

        /// <summary>
        /// 隐藏子panel
        /// </summary>
        /// <param name="panel"></param>
        public void HideSubPanel(BasePanel panel)
        {
            if (panel != null)
            {
                DoHidePanel(panel);
            }
        }

        /// <summary>
        /// 显示或隐藏GameObject
        /// </summary>
        /// <param name="viewObj"></param>
        /// <param name="isShow"></param>
        public void ViewGameObjectShow(GameObject viewObj, bool isShow)
        {
            if (viewObj == null) { return; }

            // 通过setActive的方式处理
            //viewObj.SetActive(isShow);

            // 通过修改层级的方式进行处理
            if (isShow)
            {
                if (viewObj.layer != LayerMask.NameToLayer("UIShow"))
                {
                    // 通过调整层级的方式显示，UIShow
                    viewObj.ChangeLayer(LayerMask.NameToLayer("UIShow"));
                }
                // 解决普通界面会被挡住点击的问题
                var Raycaster = viewObj.GetComponent<GraphicRaycaster>();
                if (Raycaster != null)
                {
                    Raycaster.enabled = true;
                }
            }
            else
            {
                if (viewObj.layer != LayerMask.NameToLayer("UIHide"))
                {
                    // 通过调整层级的方式显示，UIHide
                    viewObj.ChangeLayer(LayerMask.NameToLayer("UIHide"));
                }
                // 解决普通界面会被挡住点击的问题
                var Raycaster = viewObj.GetComponent<GraphicRaycaster>();
                if (Raycaster != null)
                {
                    Raycaster.enabled = false;
                }
            }
        }

        /// <summary>
        /// 获取显示的panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BasePanel GetShowPanel<T>() where T : BasePanel
        {
            // 去显示列表找
            return showPanelList.FindLast((panel) => !panel.isBeShow && panel is T);
        }

        /// <summary>
        /// 判断界面是否在显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsPanelShow<T>() where T : BasePanel
        {
            // 去显示列表找
            return showPanelList.FindLastIndex((panel) => panel.isShow && panel is T) > -1;
        }


        /// <summary>
        /// 回到主页
        /// </summary>
        public void BackToBase()
        {
            // 把base上面的界面全部清掉，只需要清空Main层
            var list = GetListByLevel(ViewCanvasLevel.MAIN);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var panel = list[i];
                ClosePanel(panel);
            }
        }

        /// <summary>
        /// 判断当前界面之上是否还有界面
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public bool HasPanleAbove(BasePanel panel, params ViewCanvasLevel[] levels)
        {
            if (panel == null) { return false; }
            // 取列表
            if (levels == null || levels.Length <= 0)
            {
                // 如果没有传，默认同层级
                levels = new ViewCanvasLevel[1] { panel.canvasLevel };
            }

            // 遍历找上层是否有界面显示
            for (int i = 0; i < levels.Length; i++)
            {
                var list = GetListByLevel(levels[i]);
                if (list.Contains(panel))
                {
                    // 如果这个层包含了指定panel，则需要从当前panel往后找
                    var startIndex = list.IndexOf(panel);
                    for (int j = startIndex + 1; j < list.Count; j++)
                    {
                        // 上层有显示的，返回
                        if (list[j].isShow)
                        {
                            return true;
                        }
                    }
                } else
                {
                    // 找出显示的界面
                    var findIndex = list.FindIndex((panel) => panel.isShow);
                    return findIndex >= 0;
                }
            }
            
            return false;
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 是否是窄屏幕，需要做适配
        /// </summary>
        public bool isAdaptWidth { get; private set; }

        /// <summary>
        /// 当前是否有弹窗在显示
        /// </summary>
        public bool hasDialogShow
        {
            get {
                // 遍历Main层，查找
                var list = GetListByLevel(ViewCanvasLevel.MAIN);
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].isShow && list[i].coverType == ViewCoverType.DAILOG)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 当前panel路径
        /// </summary>
        public string curPanelPath
        {
            get
            {
                var pathString = "";

                // 遍历Base层，查找
                var list = GetListByLevel(ViewCanvasLevel.BASE);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].isShow || list[i].isBeHide)
                    {
                        pathString += list[i].PanelName + "/";
                    }
                }

                // 遍历Main层，查找
                list = GetListByLevel(ViewCanvasLevel.MAIN);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].isShow || list[i].isBeHide)
                    {
                        pathString += list[i].PanelName + "/";
                    }
                }

                return pathString;
            }
        }

        #endregion
    }
}