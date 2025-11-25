using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFrame
{
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
}
