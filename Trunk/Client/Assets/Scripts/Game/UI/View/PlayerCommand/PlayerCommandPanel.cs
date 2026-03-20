using UnityEngine;
using Dialogue;
using System.Collections.Generic;
using FileIO;
using UnityEngine.UI;
using WNGameBase;
using WNEngine;
using static UnityEditor.Progress;

namespace UIFrame
{
    public class PlayerCommandPanel : Panel
    {
        #region 基础方法
        public override ViewCanvasLevel canvasLevel => ViewCanvasLevel.MAIN;
        public override ViewCoverType coverType => ViewCoverType.DAILOG;
        public override void OnCreate() => View = new PlayerCommandView();
        public override string resPath => "Prefabs/UI/PlayerCommand/PlayerCommand";
        #endregion

        #region 基础数据
        private PlayerCommandView view;
        // 一般来说UI界面是管不到WNGame这里的，但是这里是GM指令，比较特殊
        private WNGame WNGame;
        #endregion

        #region 生命周期
        public override void OnShow()
        {

        }

        public override void OnHide()
        {

        }

        public override void OnClose()
        {
            view.GMInputField.onSubmit.RemoveListener(ProcessPlayerCommand);
        }

        public override void OnBeforeShow()
        {

        }

        public override void OnViewLoaded()
        {
            WNGame = WNGame.Instance;
            view = panleView as PlayerCommandView;

            view.GMInputField.onSubmit.AddListener(ProcessPlayerCommand);
        }
        #endregion

        #region 相关逻辑
        /// <summary>
        /// 处理GM指令，级别最高放在WNGame中
        /// </summary>
        /// <param name="command"></param>
        public virtual void ProcessPlayerCommand(string command)
        {
            if (command.ToLower().StartsWith("switchscene"))
            {
                string[] getParam = command.Split(new char[1] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (getParam.Length > 1)
                {
                    if (getParam[1] == "Scene_Farm") //switchscene Scene_Farm
                    {
                        WNGame.GameBuilder.MapSceneName = getParam[1];
                        WNGame.GameBuilder.SceneLoader.BuildMapScene(WNGame.GameBuilder.MapSceneName);
                        Debug.Log("[aoandouli] PlayerCommandPanel.ProcessPlayerCommand switchscene Scene_Farm.");
                    }
                    else if (getParam[1] == "Scene_Farm_1") //switchscene Scene_Farm_1
                    {
                        WNGame.GameBuilder.MapSceneName = getParam[1];
                        WNGame.GameBuilder.SceneLoader.BuildMapScene(WNGame.GameBuilder.MapSceneName);
                        Debug.Log("[aoandouli] PlayerCommandPanel.ProcessPlayerCommand switchscene Scene_Farm_1.");
                    }
                }
            }
            else if (command.ToLower().StartsWith("switchitem")) //switchitem + itemid
            {
                string[] getParam = command.Split(new char[1] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (getParam.Length > 1)
                {
                    int itemid = 0;
                    if (int.TryParse(getParam[1], out itemid))
                    {
                        if (itemid > 1000)
                        {
                            WNGame.InventoryManager.SwitchItem(getParam[1]);
                            Debug.Log("[aoandouli] PlayerCommandPanel.ProcessPlayerCommand switchitem : " + itemid);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
