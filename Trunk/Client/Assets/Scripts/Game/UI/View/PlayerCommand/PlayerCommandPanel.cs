using UnityEngine;
using Dialogue;
using System.Collections.Generic;
using FileIO;
using UnityEngine.UI;
using WNGameBase;
using WNEngine;
using System;
using UnityEngine.Windows;

namespace UIFrame
{
    public class PlayerCommandPanel : Panel
    {
        #region 基础方法
        public override ViewCanvasLevel canvasLevel => ViewCanvasLevel.MAIN;
        public override ViewCoverType coverType => ViewCoverType.IN_CONTAINER;
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
        /// 输入的GM指令请全部用英文输入
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
                        Item item = new Item();
                        item.Init(WNGame.GameBuilder.ContainsItemDetails(getParam[1]));
                        WNGame.InventoryManager.SwitchItem(item);
                        Debug.Log("[aoandouli] PlayerCommandPanel.ProcessPlayerCommand switchitem : " + itemid);
                    }
                }
            }
            else if (command.ToLower().StartsWith("createitem")) //createitem + Itemid + 位置     例：createitem 1002 (0,0) createitem 1001 (0,0)
            {
                string[] getParam = command.Split(new char[1] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (getParam.Length >= 3)
                {
                    int itemid = 0;
                    // 要判断是否能转为int
                    if (int.TryParse(getParam[1], out itemid))
                    {
                        string[] posInput = getParam[2].Replace("(", "").Replace(")", "").Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                        // 转换为浮点数数组
                        float[] floatValues = Array.ConvertAll(posInput, float.Parse);
                        if (floatValues.Length >= 2)
                        {
                            Vector2 pos = new Vector2(floatValues[0], floatValues[1]);
                            WNGame.GameBuilder.CreateItem(getParam[1], pos);
                            Debug.Log("[aoandouli] PlayerCommandPanel.ProcessPlayerCommand createitem : " + itemid);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
