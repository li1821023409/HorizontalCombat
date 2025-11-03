using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimeEvent
{
    public class TimerData
    {
        public float delay { get; private set; } //延迟时间
        public Action callback { get; private set; } //回调方法
        public bool repeat { get; private set; } //是否重复调用
        public TimerData(float delay, Action callback, bool repeat = false)
        {
            this.delay = delay;
            this.callback = callback;
            this.repeat = repeat;
        }
    }
    //定时器脚本
    public class Timer
    {
        //定时器字典 存储回调内容
        private List<TimerData> _eventList = new List<TimerData>();
        private List<float> _timerList = new List<float>();

        /// <summary>
        /// 初始化定时器
        /// </summary>
        /// <param name="delay">倒计时时间</param>
        /// <param name="callback">回调方法</param>
        /// <param name="repeat">是否重复(循环)调用</param>
        public Timer(float delay, Action callback, bool repeat = false)
        {
            this.AddListener(delay, callback, repeat);
        }
        /// <summary>
        /// 空的构造函数时需要自己手动 Add
        /// </summary>
        public Timer() { }

        /// <summary>
        /// 添加定时器
        /// </summary>
        /// <param name="delay">倒计时时间</param>
        /// <param name="callback">回调方法</param>
        /// <param name="repeat">是否重复(循环)调用</param>
        public void AddListener(float delay, Action callback, bool repeat = false)
        {
            TimerData timeO = new TimerData(delay, callback, repeat);
            if (!this._eventList.Equals(timeO))
            {
                this._eventList.Add(timeO);
                _timerList.Add(0f);
            }

        }
        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="timerOwner"></param>
        private void RemoveListener(TimerData timerOwner)
        {
            if (!this._eventList.Equals(timerOwner))
            {
                this._eventList.Remove(timerOwner);
            }
        }
        /// <summary>
        /// 开始定时器
        /// </summary>
        public void Start()
        {
            if (_eventList.Count < 1)
            {
                Debug.Log("暂无任何定时器!");
            }
            else
            {
                TimerCall.Instance.Add(this);
            }
        }
        /// <summary>
        /// 暂停定时器
        /// </summary>
        public void Stop()
        {
            if (_eventList.Count < 1)
            {
                Debug.Log("暂无任何定时器!");
            }
            else
            {
                this.Clear();
            }
        }


        /// <summary>
        /// 清理定时器
        /// </summary>
        private void Clear()
        {
            TimerCall.Instance.Remove(this);
            this._eventList.Clear();
            this._timerList.Clear();
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _eventList.Count; i++)
            {
                TimerData timerOwner = _eventList[i];
                _timerList[i] += deltaTime;
                if (_timerList[i] >= timerOwner.delay)
                {
                    timerOwner.callback?.Invoke();
                    if (!timerOwner.repeat)
                    {
                        this.RemoveListener(timerOwner);
                        this._timerList.RemoveAt(i);
                    }
                    else
                    {
                        if (_timerList.Count > i)
                            _timerList[i] = 0;
                    }
                }
            }
        }
    }

    public class TimerCall : UnitySingleton<TimerCall>
    {
        private List<Timer> _timerEvent = new List<Timer>();
        public void Add(Timer timerEvent)
        {
            if (!this._timerEvent.Equals(timerEvent))
                this._timerEvent.Add(timerEvent);
        }
        public void Remove(Timer timerEvent)
        {
            for (int i = 0; i < this._timerEvent.Count; i++)
            {
                if (this._timerEvent[i] == timerEvent)
                {
                    this._timerEvent.RemoveAt(i);
                }
            }
        }

        public void RemoveAll()
        {
            this._timerEvent.Clear();
        }

        void Update()
        {
            for (int i = 0; i < _timerEvent.Count; i++)
            {
                this._timerEvent[i]?.Update(Time.deltaTime);
            }
        }
    }
}

