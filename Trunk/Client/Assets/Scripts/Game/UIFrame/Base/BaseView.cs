// Create by DongShengLi At 2024/5/10

using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UIFrame
{
    /// <summary>
    /// 所有视图的基类，担当MVP框架的View，视图部分，被动的，不与model直接接触
    /// </summary>
    public abstract class BaseView
    {
        /// <summary>
        /// 是否已经初始化了
        /// </summary>
        public bool hasInited { get; private set; }

        /// <summary>
        /// 视图关联的GameObject
        /// </summary>
        public GameObject viewGameObject;

        /// <summary>
        /// 初始化方法，必须要有gameobject传入
        /// </summary>
        /// <param name="obj"></param>
        public void InitView(GameObject obj)
        {
            viewGameObject = obj;
            // 去找到所有组件
            FindAllChildren(obj);
            // 初始化完成
            hasInited = true;
            // 触发方法
            OnAttach();
        }

        /// <summary>
        /// 清空视图，删除Gameobject
        /// </summary>
        public void ClearView()
        {
            // 触发方法
            OnDetach();
            // 解除关联
            viewGameObject = null;
        }

        /// <summary>
        /// 关联到gameobject后触发一次
        /// </summary>
        protected virtual void OnAttach() { }

        /// <summary>
        /// gameobject移除销毁前触发一次
        /// </summary>
        protected virtual void OnDetach() { }

        #region 找子组件功能相关

        private Dictionary<string, Transform> _UI;
        public Dictionary<string, Transform> UI
        {
            private set { }
            get
            {
                if (_UI == null)
                {
                    _UI = new Dictionary<string, Transform>();
                }
                return _UI;
            }
        }

        private void FindAllChildren(GameObject obj)
        {
            // 使用递归遍历节点，直到全部找到
            FindTargetByRecursion(obj.transform, UI);
        }

        /// <summary>
        /// 递归遍历将所有节点都加入到列表
        /// </summary>
        /// <param name="tr">目标transform</param>f
        /// <param name="UI">存放UI的字典</param>
        private void FindTargetByRecursion(Transform tr, Dictionary<string, Transform> UI)
        {
            foreach (Transform child in tr)
            {
                // 节点名称匹配，有效节点加入字典
                if (Regex.IsMatch(child.name, UIConst.FIND_MATCH))
                {
                    if (!UI.ContainsKey(child.name))
                    {
                        UI.Add(child.name, child);
                    }
                }
                // 继续往下找
                FindTargetByRecursion(child, UI);
            }
        }

        /// <summary>
        /// 根据节点名称获得UI
        /// </summary>
        /// <typeparam name="T">节点内有的组件</typeparam>
        /// <param name="uiName"></param>
        /// <returns></returns>
        /// <exception cref="UIContFindException">找不到则抛出异常</exception>
        public T GetUI<T>(string uiName) where T : Behaviour
        {
            if (viewGameObject == null)
            {
                throw new UIFrameException($"无法获取{uiName}, 清先调用InitView(GameObject obj)进行初始化");
            }

            // 找节点
            if (UI.ContainsKey(uiName))
            {
                return UI[uiName].GetComponent<T>();
            }
            else
            {
                throw new UIFrameException($"根据{uiName}，无法找到组件{typeof(T)}，请确认名称是否包含前后空格");
            }
        }

        /// <summary>
        /// 根据节点名称获得UI
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        /// <exception cref="UIContFindException">找不到则抛出异常</exception>
        public Transform GetUI(string uiName)
        {
            if (viewGameObject == null)
            {
                throw new UIFrameException($"无法获取{uiName}, 清先调用InitView(GameObject obj)进行初始化");
            }

            // 找节点
            if (UI.ContainsKey(uiName))
            {
                return UI[uiName];
            }
            else
            {
                throw new UIFrameException($"根据{uiName}，无法找到gameobject，请确认名称是否包含前后空格");
            }
        }

        #endregion
    }
}


