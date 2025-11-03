using System;
using System.Collections.Generic;

namespace WNEngine
{
    public class BaseInfo
    {
        // The best size should be greater than 16, which is the fastlist internal default init size.
        private const int DEFAULT_LIST_SIZE = 16;
        public List<int> m_IntPropertyList = new List<int>(DEFAULT_LIST_SIZE);
        public List<long> m_LongPropertyList = new List<long>(DEFAULT_LIST_SIZE);
        public List<bool> m_BoolPropertyList = new List<bool>(DEFAULT_LIST_SIZE);
        public List<float> m_FloatPropertyList = new List<float>(DEFAULT_LIST_SIZE);
        public List<string> m_StringPropertyList = new List<string>(DEFAULT_LIST_SIZE);
        public List<Object> m_RefPropertyList = new List<Object>(DEFAULT_LIST_SIZE);

        /// <summary>
        /// 暂时用不到，以后优化
        /// </summary>
        public virtual string ConfigPath
        {
            get
            {
                return "";
            }
        }

        public bool HasProperty(int key)
        {
            return key >= 0;
        }

        public void AddProperty(ref int key, bool property)
        {
            m_BoolPropertyList.Add(property);
            key = m_BoolPropertyList.Count - 1;
        }

        public void AddProperty(ref int key, int property)
        {
            m_IntPropertyList.Add(property);
			key = m_IntPropertyList.Count - 1;
        }

        public void AddProperty(ref int key, long property)
        {
            m_LongPropertyList.Add(property);
			key = m_LongPropertyList.Count - 1;
        }

        public void AddProperty(ref int key, float property)
        {
            m_FloatPropertyList.Add(property);
            key = m_FloatPropertyList.Count - 1;
        }

        public void AddProperty(ref int key, string property)
        {
            m_StringPropertyList.Add(property);
            key = m_StringPropertyList.Count - 1;
        }

		public void AddProperty<T>(ref int key, T property) where T:class 
		{
			m_RefPropertyList.Add(property);
			key = m_RefPropertyList.Count - 1;
		}


        public long GetLongProperty(int key)
        {
            return m_LongPropertyList[key];
        }

        public ulong GetULongProperty(int key)
        {
            return (ulong)m_LongPropertyList[key]; 
        }

        public void SetProperty(int key, long property)
        {
            m_LongPropertyList[key] = property;
        }

        public void SetProperty(int key, ulong property)
        {
            m_LongPropertyList[key] = (long)property;
        }

        public int GetIntProperty(int key)
        {
            return m_IntPropertyList[key];
        }

        public uint GetUIntProperty(int key)
        {
            return (uint)m_IntPropertyList[key];
        }

        public void SetProperty(int key, int property)
        {
            m_IntPropertyList[key] = property;
        }

        public bool GetBoolProperty(int key)
        {
            return m_BoolPropertyList[key];
        }

        public void SetProperty(int key, bool property)
        {
            m_BoolPropertyList[key] = property;
        }

        public float GetFloatProperty(int key)
        {
            return m_FloatPropertyList[key];
        }

        public void SetProperty(int key, float property)
        {
            m_FloatPropertyList[key] = property;
        }

        public string GetStringProperty(int key)
        {
            return m_StringPropertyList[key];
        }

        public void SetProperty(int key, string property)
        {
            m_StringPropertyList[key] = property;
        }

        public void SetProperty<T>(int key, T property) where T : class
        {
            m_RefPropertyList[key] = property;
        }

        public T GetRefProperty<T>(int key) where T : class
        {
            return (T)m_RefPropertyList[key];
        }
    }
}
