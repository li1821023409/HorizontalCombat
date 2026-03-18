
/*这个枚举用于创建游戏中的相关资产*/

/// <summary>
/// 资产类型枚举
/// </summary>
public enum AssetTypeEnum
{
    None,
    Instances,
    BoolCanPlaceFumiture,
    BoolCanFropItemPool,
    BoolDiggable,
    BoolPath,
    BoolNpcObstacie,
    EffectInfo = 20, // 特效比较特殊，特殊处理一下
    Dialogue = 50,  // 从这里开始，后面的资产类型都不需要实例化
}

/*这个ui枚举用于相应游戏中的相关UI事件*/

/// <summary>
/// UI事件枚举
/// </summary>
public enum UIEvent
{
    PANEL_SHOW = 0, // Panel显示
    PANEL_HIDE, // Panel隐藏
    PANEL_CLOSE, // Panel关闭
    BUTTON_CLICK, // 按钮点击
    SCROLL_VIEW_SCROLL, // 滚动条滚动
    TOGGLE_VALUE_CHANGE, // toggle值变化
    SLIDE_VALUE_CHANGE, // slide值变化
                        // 这里往下就是游戏运行的相关逻辑
    NotifyInitialPanel = 100, // 初始化游戏开始界面
    NotifyDialogueRootPanel, // 初始化游戏开始界面
}

/*这个ui枚举仅用于界面创建*/

/// <summary>
/// Canvas层级枚举
/// </summary>
public enum ViewCanvasLevel
{
    NONE = -1,
    // 基础层级，一般放入常驻底部内容
    BASE = 0,
    // 常用业务层，一般放入1级全屏界面，2级弹窗，3级弹窗
    MAIN = 1000,
    // 提示层级，一般放入一些提示
    TOAST = 6000,
    // Loading层级，一般放入加载界面
    LOADING = 8000,
}

/// <summary>
/// panel铺满类型
/// </summary>
public enum ViewCoverType
{
    // 铺满全屏，会把底下的界面全挡住
    FULLSCREEN = 0,
    // 不铺满，不会挡住底下界面
    DAILOG = 1,
    // 放到容器当中了，由容器控制
    IN_CONTAINER = 2
}

/// <summary>
/// panel销毁时机
/// </summary>
public enum PanelDestoryType
{
    // 自动销毁，多少秒之后如果没有重新打开则销毁
    AUTO = 0,
    // 从不销毁
    NEVER = 1,
    // 一关闭就释放
    IMMEDIATELY = 2,
    // 跟随父Panel
    FOLLOW_PARENT = 3,
}

/// <summary>
/// 物品类型枚举
/// </summary>
public enum ItemType
{
    None = 0,
    Seed,       // 种子
    Commodity,  // 商品
    Watering_tool,  // 浇水工具
    Hoeing_tool,    // 锄地
    Chopping,       // 切割
    Breaking,       // 破坏
    Reaping,        // 收割
    Collecting,     // 收集
    Reapable,       // 可再生
    Count,          // 计数
}