using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WNGameBase
{
    /// <summary>
    /// 所有可以受伤的脚本基类
    /// </summary>
    public class AttackableTarget : MonoBehaviour
    {
        /// <summary>
        /// 生命值
        /// </summary>
        public int m_HealthPoint;

        protected SpriteRenderer m_Renderer;
    }
}
