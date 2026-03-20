using UnityEngine;
using UnityEngine.UI;

namespace UIFrame
{
    public class PlayerCommandView : BaseView
    {
        public InputField GMInputField => GetUI<InputField>("GMInputField");
    }
}