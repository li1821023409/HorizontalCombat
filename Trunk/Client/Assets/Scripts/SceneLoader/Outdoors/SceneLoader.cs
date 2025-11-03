using System.Collections;
using System.Collections.Generic;
using UIFrame;
using UnityEngine;
using WNGameBase;

public class SceneLoader : MonoBehaviour
{
    private GameInfo gameInfo;

    void Start()
    {
        BuilderGameInfo();
    }

    /// <summary>
    /// 游戏基本信息构建
    /// </summary>
    public void BuilderGameInfo()
    {
        if (gameInfo == null)
            gameInfo = GameInfo.Instance;
        gameInfo.Init();
    }
}
