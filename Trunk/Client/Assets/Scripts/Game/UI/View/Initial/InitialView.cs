using UnityEngine;
using UIFrame;
using UnityEngine.UI;

namespace UIFrame
{
    public class InitialView : BaseView
    {
        public Text MainText => GetUI<Text>("MainText");
        public GameObject Main => GetUI("Main").gameObject;
        public Transform Text => GetUI("Text");

        protected override void OnAttach()
        {
        }
    }
}
