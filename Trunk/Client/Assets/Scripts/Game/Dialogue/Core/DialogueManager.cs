using UIFrame;
using UnityEngine;

namespace Dialogue
{
    public class DialogueManager : BaseManager<DialogueManager>
    {
        /// <summary>
        /// 对话容器
        /// </summary>
        [SerializeField]
        public DialogueContainer m_DialogueContainer = new DialogueContainer();

        public override void Init()
        {

        }

        public void SetDialogueContainer(DialogueContainer dialogueContainer)
        {
            m_DialogueContainer = dialogueContainer;
        }
    }
}
