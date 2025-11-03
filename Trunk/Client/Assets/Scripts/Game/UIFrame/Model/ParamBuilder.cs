// Create by DongShengLi At 2024/6/14
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using Object = System.Object;

namespace UIFrame
{
    /// <summary>
    /// 属性创建器
    /// </summary>
    public class ParamBuilder
    {
        // 计数
        private int count;

        /// <summary>
        /// int字典
        /// </summary>
        public Dictionary<string, int> intDic;

        /// <summary>
        /// float字典
        /// </summary>
        public Dictionary<string, float> floatDic;

        /// <summary>
        /// bool字典
        /// </summary>
        public Dictionary<string, bool> boolDic;

        /// <summary>
        /// string字典
        /// </summary>
        public Dictionary<string, string> stringDic;

        /// <summary>
        /// object字典
        /// </summary>
        public Dictionary<string, Object> objDic;

        /// <summary>
        /// action字典
        /// </summary>
        public Dictionary<string, UnityAction> actionDic;

        /// <summary>
        /// 添加Int参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ParamBuilder AppendInt(int value, string key = null)
        {
            if (intDic == null)
            {
                intDic = new Dictionary<string, int>();
            }
            if (key == null)
            {
                key = count.ToString();
                count++;
            }
            intDic.Add(key, value);
            return this;
        }

        /// <summary>
        /// 添加float参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ParamBuilder AppendFloat(float value, string key = null)
        {
            if (floatDic == null)
            {
                floatDic = new Dictionary<string, float>();
            }
            if (key == null)
            {
                key = count.ToString();
                count++;
            }
            floatDic.Add(key, value);
            return this;
        }

        /// <summary>
        /// 添加bool参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ParamBuilder AppendBool(bool value, string key = null)
        {
            if (boolDic == null)
            {
                boolDic = new Dictionary<string, bool>();
            }
            if (key == null)
            {
                key = count.ToString();
                count++;
            }
            boolDic.Add(key, value);
            return this;
        }

        /// <summary>
        /// 添加string参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ParamBuilder AppendString(string value, string key = null)
        {
            if (value == null) { return this; }
            if (stringDic == null)
            {
                stringDic = new Dictionary<string, string>();
            }
            if (key == null)
            {
                key = count.ToString();
                count++;
            }
            stringDic.Add(key, value);
            return this;
        }

        /// <summary>
        /// 添加Object参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ParamBuilder AppendObject(Object value, string key = null)
        {
            if (value == null) { return this; }
            if (objDic == null)
            {
                objDic = new Dictionary<string, Object>();
            }
            if (key == null)
            {
                key = count.ToString();
                count++;
            }
            objDic.Add(key, value);
            return this;
        }

        /// <summary>
        /// 添加Actioin参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ParamBuilder AppendAction(UnityAction value, string key = null)
        {
            if (value == null) { return this; }
            if (actionDic == null)
            {
                actionDic = new Dictionary<string, UnityAction>();
            }
            if (key == null)
            {
                key = count.ToString();
                count++;
            }
            actionDic.Add(key, value);
            return this;
        }

        /// <summary>
        /// 构建参数
        /// </summary>
        /// <returns></returns>
        public Param Build()
        {
            var param = new Param(count, intDic, floatDic, boolDic, stringDic, objDic, actionDic);
            return param;
        }
    }

    /// <summary>
    /// 自定义参数
    /// </summary>
    public class Param : IDisposable
    {
        /// <summary>
        /// 参数列表包含数量
        /// </summary>
        public int count { get; private set; }
        private Dictionary<string, int> intDic;
        private Dictionary<string, float> floatDic;
        private Dictionary<string, bool> boolDic;
        private Dictionary<string, string> stringDic;
        private Dictionary<string, Object> objDic;
        private Dictionary<string, UnityAction> actionDic;

        public Param(int count, Dictionary<string, int> intDic, Dictionary<string, float> floatDic, Dictionary<string, bool> boolDic,
            Dictionary<string, string> stringDic, Dictionary<string, Object> objDic, Dictionary<string, UnityAction> actionDic)
        {
            this.count = count;
            this.intDic = intDic;
            this.floatDic = floatDic;
            this.boolDic = boolDic;
            this.stringDic = stringDic;
            this.objDic = objDic;
            this.actionDic = actionDic;
        }

        /// <summary>
        /// 获取int参数
        /// </summary>
        /// <param name="index">存入时候的次序</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public int GetInt(int index)
        {
            return GetInt(index.ToString());
        }

        /// <summary>
        /// 获取int参数
        /// </summary>
        /// <param name="key">存入时候的key</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public int GetInt(string key)
        {
            if (intDic != null && intDic.ContainsKey(key))
            {
                return intDic[key];
            }
            return 0;
        }

        /// <summary>
        /// 获取float参数
        /// </summary>
        /// <param name="index">存入时候的次序</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public float GetFloat(int index)
        {
            return GetFloat(index.ToString());
        }

        /// <summary>
        /// 获取float参数
        /// </summary>
        /// <param name="key">存入时候的key</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public float GetFloat(string key)
        {
            if (floatDic != null && floatDic.ContainsKey(key))
            {
                return floatDic[key];
            }
            return 0;
        }

        /// <summary>
        /// 获取bool参数
        /// </summary>
        /// <param name="index">存入时候的次序</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public bool GetBool(int index)
        {
            return GetBool(index.ToString());
        }

        /// <summary>
        /// 获取bool参数
        /// </summary>
        /// <param name="key">存入时候的key</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public bool GetBool(string key)
        {
            if (boolDic != null && boolDic.ContainsKey(key))
            {
                return boolDic[key];
            }
            return false;
        }

        /// <summary>
        /// 获取string参数
        /// </summary>
        /// <param name="index">存入时候的次序</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public string GetString(int index)
        {
            return GetString(index.ToString());
        }

        /// <summary>
        /// 获取string参数
        /// </summary>
        /// <param name="key">存入时候的key</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public string GetString(string key)
        {
            if (stringDic != null && stringDic.ContainsKey(key))
            {
                return stringDic[key];
            }
            return null;
        }

        /// <summary>
        /// 获取Object参数
        /// </summary>
        /// <param name="index">存入时候的次序</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public T GetObject<T>(int index) where T : class
        {
            return GetObject<T>(index.ToString());
        }

        /// <summary>
        /// 获取Object参数
        /// </summary>
        /// <param name="key">存入时候的key</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public T GetObject<T>(string key) where T : class
        {
            if (objDic != null && objDic.ContainsKey(key))
            {
                try
                {
                    return objDic[key] as T;
                }
                catch (System.Exception ex)
                {
                    throw new UIFrameException("GetObject，指定泛行无法转换，请检查传入的类型是否一致");
                }
            }
            return null;
        }

        /// <summary>
        /// 获取Action参数
        /// </summary>
        /// <param name="index">存入时候的次序</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public UnityAction GetAction(int index)
        {
            return GetAction(index.ToString());
        }

        /// <summary>
        /// 获取Action参数
        /// </summary>
        /// <param name="key">存入时候的key</param>
        /// <returns></returns>
        /// <exception cref="UIFrameException"></exception>
        public UnityAction GetAction(string key)
        {
            if (actionDic != null && actionDic.ContainsKey(key))
            {
                return actionDic[key];
            }
            return null;
        }

        public void Dispose()
        {
            if (intDic != null) { intDic.Clear(); }
            if (floatDic != null) { floatDic.Clear(); }
            if (boolDic != null) { boolDic.Clear(); }
            if (stringDic != null) { stringDic.Clear(); }
            if (objDic != null) { objDic.Clear(); }
            if (actionDic != null) { actionDic.Clear(); }
            count = 0;
        }
    }
}