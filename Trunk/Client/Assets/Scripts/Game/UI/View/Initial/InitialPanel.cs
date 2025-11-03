using UnityEngine;
using UIFrame;

namespace UIFrame
{
    public class InitialPanel : Panel
    {
        #region ��������
        public override ViewCanvasLevel canvasLevel => ViewCanvasLevel.BASE;
        public override ViewCoverType coverType => ViewCoverType.FULLSCREEN;
        public override void OnCreate() => View = new InitialView();
        public override string resPath => "Prefabs/UI/Initial/InitialPanel";
        #endregion

        #region ��������
        private InitialView view;
        private InitialManager manager;
        #endregion

        #region ��������
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
            manager = InitialManager.Instance;
            view = panleView as InitialView;

            view.Main.SetActive(true);
            view.Text.position = new Vector3(10f, 10f, 10f);
        }
        #endregion
    }
}
