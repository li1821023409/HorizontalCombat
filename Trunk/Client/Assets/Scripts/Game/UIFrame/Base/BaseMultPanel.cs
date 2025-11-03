// Create by DongShengLi At 2024/5/10
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

namespace UIFrame
{
    /// <summary>
    /// 复合面板，包含多个子面板
    /// </summary>
    public abstract class BaseMultPanel : BasePanel
    {
        /// <summary>
        /// 关联的子界面
        /// </summary>
        private Dictionary<string, BasePanel> children;

        /// <summary>
        /// 当前显示的子界面
        /// </summary>
        private List<string> curShowPanelKeys;

        #region 私有方法

        /// <summary>
        /// 显示子界面
        /// </summary>
        /// <param name="key"></param>
        private void DoShowSubPanel(string key)
        {
            // 找到关联Panel
            if (children == null || !children.ContainsKey(key)) { return; }

            // 显示子界面
            var panel = children[key];
            UIManager.Instance.ShowSubPanel(panel);
        }

        /// <summary>
        /// 隐藏子界面
        /// </summary>
        /// <param name="key"></param>
        private void DoHideSubPanel(string key)
        {
            // 找到关联Panel
            if (children == null || !children.ContainsKey(key)) { return; }

            // 隐藏子界面
            var panel = children[key];
            UIManager.Instance.HideSubPanel(panel);
        }

        /// <summary>
        /// 关闭子界面
        /// </summary>
        /// <param name="key"></param>
        private void DoCloseSubPanel(string key)
        {
            // 找到关联Panel
            if (children == null || !children.ContainsKey(key)) { return; }

            // 关闭子界面
            var panel = children[key];
            UIManager.Instance.ClosePanel(panel);
        }

        #endregion

        #region 子类方法
        /// <summary>
        /// 显示子界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="param"></param>
        protected void ShowSubPanel<T>(Transform container, Param param = null) where T : BasePanel
        {
            // 建立关联关系
            if (children == null) { children = new Dictionary<string, BasePanel>(); }
            if (curShowPanelKeys == null) { curShowPanelKeys = new List<string>(); }

            string key = container.name + typeof(T).ToString();
            // 如果子界面已经在显示了，不再处理
            if (curShowPanelKeys.Contains(key))
            {
                return;
            }
            else
            {
                curShowPanelKeys.Add(key);
            }
             
            // 加载显示界面
            UIManager.Instance.ShowSubPanel<T>(container, param);
            // 加入子列表
            if (!children.ContainsKey(key))
            {
                var subPanel = UIManager.Instance.GetShowPanel<T>();
                // 修改界面销毁时机
                subPanel.hasParent = true;
                children.Add(key, subPanel);
            }
        }

        /// <summary>
        /// 隐藏子界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        protected void HideSubPanel<T>(Transform container) where T : BasePanel
        {
            // 找到关联Panel
            string key = container.name + typeof(T).ToString();
            if (children == null || !children.ContainsKey(key)) { return; }

            // 隐藏子界面
            var panel = children[key];
            UIManager.Instance.HideSubPanel(panel);
            if (curShowPanelKeys.Contains(key)) { curShowPanelKeys.Remove(key); }
        }

        #endregion

        #region 生命周期方法

        /// <summary>
        /// 子类重写OnShow，必须添加base.OnShow()
        /// </summary>
        public override void OnShow()
        {
            if (children == null || children.Count <= 0)
            {
                return;
            }
            // 把当前还在显示的Panel都隐藏
            foreach (var item in children)
            {
                if (curShowPanelKeys.Contains(item.Key))
                {
                    DoShowSubPanel(item.Key);
                }
                else
                {
                    UIManager.Instance.ViewGameObjectShow(item.Value.viewGameObject, false);
                }
            }
        }

        /// <summary>
        /// 子类重写OnHide，必须添加base.OnHide()
        /// </summary>
        public override void OnHide()
        {
            if (curShowPanelKeys == null || curShowPanelKeys.Count <= 0)
            {
                return;
            }
            // 把当前还在显示的Panel都隐藏
            foreach (var item in curShowPanelKeys)
            {
                DoHideSubPanel(item);
            }
        }

        /// <summary>
        /// 子类重写OnClose，必须添加base.OnClose()
        /// </summary>
        public override void OnClose()
        {
            curShowPanelKeys.Clear();
            // 关闭全部子界面
            if (children == null || children.Count <= 0)
            {
                return;
            }
            // 把当前还在显示的Panel都隐藏
            foreach (var item in children)
            {
                DoCloseSubPanel(item.Key);
            }
            children.Clear();
        }

        #endregion
    }
}