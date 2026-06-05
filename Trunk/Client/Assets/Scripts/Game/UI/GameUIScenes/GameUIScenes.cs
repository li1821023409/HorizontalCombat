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
        public UIEventManager UIEventManager;
        public InitialPanel InitialPanel;
        public DialogueRootPanel DialogueRootPanel;
        public PlayerCommandPanel PlayerCommandPanel;
        public InventoryBarPanel InventoryBarPanel;

        /// <summary>
        /// 初始化部分数据
        /// </summary>
        public void Init()
        {
            CreateUIPanel();
            AddListener();
        }

        public void OnDestroy()
        {
            RemoveListener();
        }

        /// <summary>
        /// 添加监听事件
        /// </summary>
        protected virtual void AddListener()
        {
            if (UIEventManager == null)
            {
                UIEventManager = UIEventManager.Instance;
            }
            //uiEventManager.AddUIEventListener(UIEvent.NotifyDialogueRootPanel, NotifyDialogueRootPanel);
            UIEventManager.AddUIEventListener(UIEvent.NotifyUpDateInventoryBar, NotifyUpDateInventoryBar);
        }


        /// <summary>
        /// 移除监听事件
        /// GameUIScenes的监听事件这里可以不用移除，该脚本会跟随场景创建和销毁，不会移除
        /// </summary>
        protected virtual void RemoveListener()
        {
            //uiEventManager.RemoveUIEventListener(UIEvent.NotifyDialogueRootPanel, NotifyDialogueRootPanel);
            UIEventManager.RemoveUIEventListener(UIEvent.NotifyUpDateInventoryBar, NotifyUpDateInventoryBar);
        }

        /// <summary>
        /// 需要初始化显示的panl放这里
        /// </summary>
        public void CreateUIPanel()
        {
            //if (m_InitialPanel == null)
            //{
            //    m_InitialPanel = UIManager.Instance.ShowPanel<InitialPanel>();
            //}

            //if (DialogueRootPanel == null)
            //{
            //    DialogueRootPanel = UIManager.Instance.ShowPanel<DialogueRootPanel>();
            //}

            if (PlayerCommandPanel == null)
            {
                PlayerCommandPanel = UIManager.Instance.ShowPanel<PlayerCommandPanel>();
            }

            if (InventoryBarPanel == null)
            {
                InventoryBarPanel = UIManager.Instance.ShowPanel<InventoryBarPanel>();
            }
        }

        public void NotifyUpDateInventoryBar(Param param)
        {
            ItemDetails itemDetails = param.GetObject<ItemDetails>("ItemDetails");
            int slotIndex = param.GetInt("SlotIndex");
            int count = param.GetInt("Count");
            InventoryBarPanel.UpDateInventoryBar(itemDetails, slotIndex, count);
        }
    }
}

