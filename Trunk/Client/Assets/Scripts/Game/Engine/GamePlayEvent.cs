using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WNGameBase
{
    /*这个ui枚举用于相应游戏中的相关游戏玩法事件*/

    /// <summary>
    /// 事件枚举
    /// </summary>
    public enum GamePlayEvent
    {
        PressNumberKeys = 0, // 按下数字键位
        ReleaseNumberKeys = 1,// 松开数字键位
    }
}
