using UnityEngine;
using TMPro;

namespace UIFrame
{
    public class DialogueRootView : BaseView
    {
        public TextMeshProUGUI NameText => GetUI<TextMeshProUGUI>("NameText");
        public TextMeshProUGUI DialogueText => GetUI<TextMeshProUGUI>("DialogueText");
    }
}