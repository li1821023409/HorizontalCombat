using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFrame;

namespace WNGameBase
{
    /// <summary>
    /// 所有响应用户界面控制的基类
    /// </summary>
    public class GameUIScenes : UnitySingleton<GameUIScenes>
    {
        public UIEventManager uiEventManager;
        public InitialPanel initialPanel;
        public DialogueRootPanel dialogueRootPanel;

        /// <summary>
        /// 初始化部分数据
        /// </summary>
        public void Init()
        {
            AddListener();
        }


        /// <summary>
        /// 添加监听事件
        /// </summary>
        protected virtual void AddListener()
        {
            if (uiEventManager == null)
            {
                uiEventManager = UIEventManager.Instance;
            }

            //uiEventManager.AddListener(UIEvent.NotifyInitialPanel, NotifyInitialPanel);

            uiEventManager.AddListener(UIEvent.NotifyDialogueRootPanel, NotifyDialogueRootPanel);
        }


        /// <summary>
        /// 移除监听事件
        /// GameUIScenes的监听事件这里可以不用移除，该脚本会跟随场景创建和销毁，不会移除
        /// </summary>
        protected virtual void RemoveListener()
        {

        }

        /// <summary>
        /// 显示初始化界面
        /// </summary>
        /// <param name="param"></param>
        public void NotifyInitialPanel(Param param)
        {
            if (initialPanel == null)
            {
                initialPanel = UIManager.Instance.ShowPanel<InitialPanel>();
            }
        }

        /// <summary>
        /// 显示对话界面
        /// </summary>
        /// <param name="param"></param>
        public void NotifyDialogueRootPanel(Param param)
        {
            if (dialogueRootPanel == null)
            {
                dialogueRootPanel = UIManager.Instance.ShowPanel<DialogueRootPanel>();
            }
        }
    }
}

