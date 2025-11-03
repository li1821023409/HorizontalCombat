// Create by DongShengLi At 2024/3/7
namespace UIFrame
{
    /// <summary>
    /// 所有Manager的基类，担当MVP框架的Model，数据逻辑部分
    /// </summary>
    public abstract class BaseManager<T> : Singleton<T> where T : new()
    {
        /// <summary>
        /// 标记
        /// </summary>
        public readonly string TAG = typeof(T).ToString();

        /// <summary>
        /// 初始化方法  
        /// </summary>
        public abstract void Init();
    }
}

