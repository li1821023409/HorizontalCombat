// Create by DongShengLi At 2024/5/10
using UnityEngine;
using System.Collections;

namespace UIFrame
{
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
}

