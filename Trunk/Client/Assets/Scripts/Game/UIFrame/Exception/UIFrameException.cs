// Create by DongShengLi At 2024/5/10
using System;
using System.Text;
// using UnityEditor.VersionControl;

namespace UIFrame
{
    /// <summary>
    /// UI框架异常
    /// </summary>
    public class UIFrameException : Exception
    {
        private string message;

        /// <summary>
        /// 自定义输出内容
        /// </summary>
        /// <param name="message"></param>
        public UIFrameException(string message) : base(message)
        {
            this.message = message;
        }

        /// <summary>
        /// 重写Tostring,
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(UIConst.EXCEPTION_HEAD);
            sb.Append(message);
            return sb.ToString();
        }
    }
}


