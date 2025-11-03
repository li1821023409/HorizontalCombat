// Create by DongShengLi At 2024/5/10
using UnityEngine;
using UnityEngine.Events;

namespace UIFrame
{
    /// <summary>
    /// 所有Panel的基类，担当MVP框架的Presenter，负责处理业务逻辑，与View-Model交互
    /// </summary>
    public abstract class BasePanel : MonoBehaviour
    {
        #region 基础属性
        /// <summary>
        /// panel名称，跟类名一致
        /// </summary>
        public string PanelName;

        /// <summary>
        /// 当前panle处在层级的排序
        /// </summary>
        public int orderIndex;

        /// <summary>
        /// Panel关联的gameObject
        /// </summary>
        public GameObject viewGameObject;

        /// <summary>
        /// 面板关联的View，必须设置
        /// </summary>
        public BaseView panleView { get; private set; }

        /// <summary>
        /// 面板所属层级，必须设置
        /// </summary>
        public abstract ViewCanvasLevel canvasLevel { get; }

        /// <summary>
        /// 面板铺满类型，必须设置
        /// </summary>
        public abstract ViewCoverType coverType { get; }

        /// <summary>
        /// 界面销毁方式，默认自动
        /// </summary>
        public virtual PanelDestoryType destoryType { get => PanelDestoryType.AUTO; }

        /// <summary>
        /// 界面预制体路径，默认为空，不为空的话会在加载时替换默认路径
        /// </summary>
        public virtual string resPath { get => null; }

        /// <summary>
        /// 界面适配手机的预制体路径，默认为空，不为空的话会在加载进行判断，如果需要替换为手机界面则会替换，否则默认
        /// </summary>
        public virtual string resPathPhone { get => null; }

        /// <summary>
        /// 界面是否在显示中
        /// </summary>
        public bool isShow;

        /// <summary>
        /// 界面是否是被动显示的，比如上层界面关掉了
        /// </summary>
        public bool isBeShow;

        /// <summary>
        /// 界面是否是被动隐藏的，比如被遮挡了
        /// </summary>
        public bool isBeHide;

        /// <summary>
        /// 视图加载完的事件，在视图加载完之后会触发一次
        /// </summary>
        public UnityEvent viewLoadedEvent = new UnityEvent();

        /// <summary>
        /// 界面关闭的协程，给UIManager调用，不需要处理
        /// </summary>
        public Coroutine panelCloseCorotine;

        /// <summary>
        /// 调用ShowPanel时传入的参数
        /// </summary>
        public Param showParam;

        /// <summary>
        /// 是否有父节点
        /// </summary>
        public bool hasParent;

        #endregion

        #region 子类方法

        /// <summary>
        /// 要求子类必须实现的方法，返回一个BaseView对象
        /// </summary>
        /// <returns></returns>
        protected abstract BaseView GetPanelView();

        #endregion

        #region 处理MonoBehaviour基础生命周期方法，不让子类直接调用

        /// <summary>
        /// 设为私有方法，子类不调用
        /// </summary>
        private void Awake() { }

        /// <summary>
        /// 设为私有方法，子类不调用
        /// </summary>
        private void Start() { }

        /// <summary>
        /// 设为私有方法，子类不调用
        /// </summary>
        private void OnEnable() { }

        /// <summary>
        /// 设为私有方法，不给子类提供使用，实在要用可以使用协程
        /// </summary>
        private void Update() { }

        /// <summary>
        /// 设为私有方法，子类不调用
        /// </summary>
        private void FixedUpdate() { }

        /// <summary>
        /// 设为私有方法，子类不调用
        /// </summary>
        private void LateUpdate() { }

        /// <summary>
        /// 设为私有方法，子类不调用
        /// </summary>
        private void OnDisable() { }

        /// <summary>
        /// 设为私有方法，子类不调用
        /// </summary>
        private void OnDestroy() { }

        #endregion

        #region 提供给子类使用的生命周期方法

        /// <summary>
        /// 脚本创建后触发，这个时候可以进行加载数据、资源的操作
        /// </summary>
        public virtual void OnCreate() { }

        /// <summary>
        /// 视图加载完毕时触发，这个时候可以使用view了，可以进行一些添加监听操作，视图初始化操作
        /// </summary>
        public abstract void OnViewLoaded();

        /// <summary>
        /// 生命周期方法，界面显示前触发，可能会多次触发，使用isBeShow判断是否是因为不再被遮挡导致的隐藏
        /// </summary>
        public virtual void OnBeforeShow() { }

        /// <summary>
        /// 生命周期方法，界面显示后触发，可能会多次触发，使用isBeShow判断是否是因为不再被遮挡导致的隐藏
        /// </summary>
        public abstract void OnShow();

        /// <summary>
        /// 生命周期方法，界面隐藏前触发，可能会多次触发，使用isBeHide判断是否是因为被遮挡导致的隐藏
        /// 返回延迟关闭时间，如果时间大于0，会执行协程，等待一定时间后隐藏界面
        /// </summary>
        /// <returns>返回关闭延迟时间</returns>
        public virtual float OnBeforeHide() { return 0; }

        /// <summary>
        /// 生命周期方法，界面隐藏后触发，可能会多次触发，使用isBeHide判断是否是因为被遮挡导致的隐藏
        /// </summary>
        public abstract void OnHide();

        /// <summary>
        /// 生命周期方法，界面关闭的时候触发，只触发一次，可以进行一些移除监听操作，数据清空操作
        /// </summary>
        public abstract void OnClose();

        /// <summary>
        /// 生命周期方法，界面销毁的时候触发
        /// </summary>
        public virtual void OnViewDestroy() { }

        #endregion

        #region 一些提供给外部使用通用方法

        /// <summary>
        /// 初始化PanelView，传入关联的GameObject
        /// </summary>
        /// <param name="obj"></param>
        public void InitPanelView(GameObject obj)
        {
            viewGameObject = obj;
            panleView = GetPanelView();
            panleView?.InitView(obj);
        }

        #endregion
    }
}