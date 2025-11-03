using UnityEngine;
using TMPro;

namespace Dialogue
{
    /// <summary>
    /// 对话容器
    /// </summary>
    [System.Serializable]
    public class DialogueContainer
    {
        /// <summary>
        /// 对话根节点
        /// 所有对话都放在根节点下
        /// </summary>
        private GameObject m_DialogueRoot;
        public GameObject DialogueRoot
        {
            set { m_DialogueRoot = value; }
            get { return m_DialogueRoot; }
        }

        /// <summary>
        /// 名字
        /// </summary>
        private TextMeshProUGUI m_NameText;
        public TextMeshProUGUI NameText
        {
            set { m_NameText = value; }
            get { return m_NameText; }
        }
        /// <summary>
        /// 对话内容
        /// </summary>
        private TextMeshProUGUI m_DialogueText;
        public TextMeshProUGUI DialogueText
        {
            set { m_DialogueText = value; }
            get { return m_DialogueText; }
        }

        public DialogueContainer()
        {

        }

        public DialogueContainer(GameObject dialogueRoot, TextMeshProUGUI nameText, TextMeshProUGUI dialogueText)
        {
            DialogueRoot = dialogueRoot;
            NameText = nameText;
            DialogueText = dialogueText;
        }
    }
}
