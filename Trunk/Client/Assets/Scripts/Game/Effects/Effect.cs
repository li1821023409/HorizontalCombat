using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeEvent;
using System.Xml.Linq;

namespace WNGameBase
{
    /// <summary>
    /// 所有特效的根脚本
    /// </summary>
    public class Effect : MonoBehaviour
    {
        protected string m_AssetID;
        protected string m_Name;
        protected float m_RunTime = 0.5f;
        protected Timer m_Timer = new Timer();
        // 持续时间是可以被赋值的
        public float RunTime
        {
            set { m_RunTime = value; }
            get { return m_RunTime; }
        }

        /// <summary>
        /// Effect初始化
        /// 设置特效持续时间
        /// </summary>
        public void InitEffect(string assetId, string naem,float runTime)
        {
            m_AssetID = assetId;
            m_Name = naem;
            m_RunTime = runTime;
            if (m_Timer != null)
            {
                // 定时器，一定时间后执行回调
                m_Timer.AddListener(m_RunTime, DestroyEffect);
                m_Timer.Start();
            }
        }

        protected virtual void DestroyEffect()
        {
            ObjectPool.Instance.ReturnToPool(m_AssetID, this.gameObject);
        }
    }
}
