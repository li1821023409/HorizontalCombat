using UnityEngine;
using Dialogue;
using System.Collections.Generic;
using FileIO;

namespace UIFrame
{
    public class DialogueRootPanel : Panel
    {
        #region 基础方法
        public override ViewCanvasLevel canvasLevel => ViewCanvasLevel.BASE;
        public override ViewCoverType coverType => ViewCoverType.FULLSCREEN;
        public override void OnCreate() => View = new DialogueRootView();
        public override string resPath => "Prefabs/UI/Dialogue/DialogueRootPanel";
        #endregion

        #region 基础数据
        private DialogueRootView view;
        private DialogueRootManager manager;
        private DialogueManager dialogueManager;

        private TextArchitect textArchitect = null;

        private List<DialogueData> lines = new List<DialogueData>();
        private string readFileName => "Dialogue";
        #endregion

        #region 生命周期
        public override void OnShow()
        {

        }

        public override void OnHide()
        {

        }

        public override void OnClose()
        {

        }

        public override void OnBeforeShow()
        {

        }

        public override void OnViewLoaded()
        {
            manager = DialogueRootManager.Instance;
            dialogueManager = DialogueManager.Instance;
            view = panleView as DialogueRootView;

            // 这里创建DialogueManager对应数据
            DialogueContainer dialogueContainer = new DialogueContainer(view.viewGameObject, view.NameText, view.DialogueText);
            dialogueManager.SetDialogueContainer(dialogueContainer);

            if (textArchitect == null)
            {
                textArchitect = new TextArchitect(view.DialogueText);
            }

            lines = FileManager.Instance.ReadCSVFilesToList<DialogueData>(readFileName);
        }

        /// <summary>
        /// 文本生成
        /// </summary>
        protected virtual void Update()
        {
            if (textArchitect != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (textArchitect.IsBuild && !textArchitect.IsHurryUp)
                    {
                        textArchitect.IsHurryUp = true;
                    }
                    else if (textArchitect.IsBuild && textArchitect.IsHurryUp)
                    {
                        textArchitect.ForceComplete();
                    }
                    else
                    {
                        textArchitect.Build(lines[Random.Range(0, lines.Count)].diaLogue);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    textArchitect.Append(lines[Random.Range(0, lines.Count)].diaLogue);
                }
            }
        }
        #endregion
    }
}
