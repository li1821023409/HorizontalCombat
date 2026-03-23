
/*该文件用于存储游戏中的通用枚举*/

/// <summary>
/// 资源类型枚举
/// </summary>
public enum AssetTypeEnum
{
    None,
    Instances,
    BoolCanPlaceFumiture, // 是否可以放置家具
    BoolCanFropItemPool,  // 是否可以丢弃物品
    BoolDiggable,         // 是否可挖掘
    BoolPath,             // 是否为路径
    BoolNpcObstacie,      // 是否为NPC障碍物
    EffectInfo = 20,      // 特效信息，暂时处理为一个类型
    Dialogue = 50,        // 对话类型，开始扩展资源类型的实现
}

/*该UI枚举主要对应游戏中的UI事件*/

/// <summary>
/// UI事件枚举
/// </summary>
public enum UIEvent
{
    PANEL_SHOW = 0,       // Panel显示
    PANEL_HIDE,           // Panel隐藏
    PANEL_CLOSE,          // Panel关闭
    BUTTON_CLICK,         // 按钮点击
    SCROLL_VIEW_SCROLL,   // 滚动视图滚动
    TOGGLE_VALUE_CHANGE,  // toggle值变化
    SLIDE_VALUE_CHANGE,   // slide值变化
                          // 以下为游戏中的通知逻辑
    NotifyInitialPanel = 100, // 初始化游戏初始面板
    NotifyDialogueRootPanel,  // 初始化游戏对话根面板
    NotifyUpDateInventoryBar,  // 初始化游戏对话根面板
}

/*该UI枚举主要用于层级创建*/

/// <summary>
/// Canvas层级枚举
/// </summary>
public enum ViewCanvasLevel
{
    NONE = -1,
    // 基础层级：一般驻留底层显示
    BASE = 0,
    // 主业务层：一般包括1.全屏面板，2.弹窗面板，3.容器面板
    MAIN = 1000,
    // 提示层级：一般包括一些提示显示
    TOAST = 6000,
    // Loading层级：一般包括加载进度
    LOADING = 8000,
}

/// <summary>
/// panel覆盖类型
/// </summary>
public enum ViewCoverType
{
    // 覆盖全屏，完全遮挡下层内容
    FULLSCREEN = 0,
    // 弹窗类型，不会遮挡底层内容
    DAILOG = 1,
    // 容器类型，嵌套显示
    IN_CONTAINER = 2
}

/// <summary>
/// panel销毁时机
/// </summary>
public enum PanelDestoryType
{
    // 自动销毁：关闭后不再保留，需要重新创建
    AUTO = 0,
    // 永不销毁
    NEVER = 1,
    // 一关闭立即释放
    IMMEDIATELY = 2,
    // 跟随父Panel
    FOLLOW_PARENT = 3,
}

/// <summary>
/// 道具类型枚举
/// </summary>
public enum ItemType
{
    None = 0,
    Seed,       // 种子
    Commodity,  // 商品
    Watering_tool,  // 浇水工具
    Hoeing_tool,    // 锄头
    Chopping,       // 砍伐
    Breaking,       // 破坏
    Reaping,        // 收割
    Collecting,     // 采集
    Reapable,       // 可收割
    Count,          // 计数
}
