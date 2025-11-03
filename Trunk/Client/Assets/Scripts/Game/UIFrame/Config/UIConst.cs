// Create by DongShengLi At 2024/5/10

namespace UIFrame
{
    /// <summary>
    /// 框架内的一些常量，可修改
    /// </summary>
    public class UIConst
    {
        /// <summary>
        /// view销毁倒计时时间
        /// </summary>
        public const int VIEW_DESTORY_INTERVAL = 10;

        /// <summary>
        /// 每个view的厚度
        /// </summary>
        public const int VIEW_THICK = 100;

        /// <summary>
        /// 用于界面查找节点的匹配正则字符串
        /// </summary>
        public const string FIND_MATCH = "^[a-zA-Z0-9]*[_[a-zA-Z0-9]*]*$";

        /// <summary>
        /// 统一异常说明头
        /// </summary>
        public const string EXCEPTION_HEAD = "UIFrame Exception:";

        /// <summary>
        /// 默认预制体加载路径
        /// </summary>
        public const string DEFAULT_RES_PATH = "GUI/Prefabs/";
    }
}
