/// <summary>
/// [aoandouli]这里使用PlayerInput也行，但是我这边选择绑定事件处理
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UIFrame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using WNGameBase;

namespace WNGameBase
{
    public class InputManager : UnitySingleton<InputManager>
    {
        /// <summary>
        /// 玩家控制的pawn对象需要获取输入参数
        /// </summary>
        public InputControl m_InputControl { get; private set; }

        protected Vector2 m_MoveVector2 = Vector2.zero;
        /// <summary>
        /// 输入向量
        /// </summary>
        public Vector2 MoveVector2
        {
            get { return m_MoveVector2; }
        }

        public bool m_IsPressNumberKeys = false;
        /// <summary>
        /// 是否按下数字键
        /// </summary>
        public bool IsPressNumberKeys
        {
            get { return m_IsPressNumberKeys; }
        }

        protected int m_PressNumberKeys = -1;
        /// <summary>
        /// 按下的数字键（需要注意的是如果是默认的-1，则视为没有按下数字键）
        /// </summary>
        public int PressNumberKeys
        {
            get { return m_PressNumberKeys; }
        }

        private static readonly Dictionary<string, int> pathToNum = new Dictionary<string, int>
        {
            { "/Keyboard/1", 1 }, { "/Keyboard/2", 2 }, { "/Keyboard/3", 3 },
            { "/Keyboard/4", 4 }, { "/Keyboard/5", 5 }, { "/Keyboard/6", 6 },
            { "/Keyboard/7", 7 }, { "/Keyboard/8", 8 }, { "/Keyboard/9", 9 },
        };

        public void Init()
        {
            m_InputControl = new InputControl();
            InputControlEnable();
        }

        //public void Tick()
        //{
        //    GetInputVlue();
        //}

        #region 绑定玩家输入
        /// <summary>
        /// 绑定输入事件
        /// </summary>
        protected void BindInputEvent()
        {
            m_InputControl.GamePlay.UseItem.performed += OnUseItem;
            m_InputControl.GamePlay.UseItem.canceled += OnUseItem;
            m_InputControl.GamePlay.Move.performed += OnInputVlue;
            m_InputControl.GamePlay.Move.canceled += OnInputVlue;
        }

        /// <summary>
        /// 移除输入事件
        /// </summary>
        protected void RemoveInputEvent()
        {
            m_InputControl.GamePlay.UseItem.performed -= OnUseItem;
            m_InputControl.GamePlay.UseItem.canceled -= OnUseItem;
            m_InputControl.GamePlay.Move.performed -= OnInputVlue;
            m_InputControl.GamePlay.Move.canceled -= OnInputVlue;
        }

        private void OnUseItem(InputAction.CallbackContext ctx)
        {
            var control = ctx.control;
            string path = control != null ? control.path : "<unknown>";
            int num;
            pathToNum.TryGetValue(path, out num); // 未找到则 num=0

            if (ctx.phase == InputActionPhase.Performed)
            {
                m_IsPressNumberKeys = num != 0;
                m_PressNumberKeys = m_IsPressNumberKeys ? num : 0;
                UIEventManager.Instance.GamePlayEventEmit(GamePlayEvent.PressNumberKeys);
            }
            else if (ctx.phase == InputActionPhase.Canceled)
            {
                m_IsPressNumberKeys = false;
                m_PressNumberKeys = 0;
                UIEventManager.Instance.GamePlayEventEmit(GamePlayEvent.ReleaseNumberKeys);
            }
        }

        // 移动相关参数
        public delegate void MovementDelegate(Vector2 vector2);
        public event MovementDelegate MovementEvent;
        protected void OnInputVlue(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Performed)
            {
                m_MoveVector2 = ctx.ReadValue<Vector2>();
            }
            else if (ctx.phase == InputActionPhase.Canceled)
            {
                m_MoveVector2 = Vector2.zero;
            }
            // 这里执行相关的移动事件
            MovementEvent(m_MoveVector2);
        }
        #endregion

        #region 输入系统的启用与结束
        protected virtual void InputControlEnable()
        {
            m_InputControl.Enable();
            BindInputEvent();
        }

        protected virtual void InputControlDisable()
        {
            RemoveInputEvent();
            m_InputControl.Disable();
        }
        #endregion
    }
}