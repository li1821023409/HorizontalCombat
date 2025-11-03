using UnityEngine;

namespace UIFrame
{
    public abstract class Panel : BasePanel
    {
        protected BaseView View;

        protected override BaseView GetPanelView()
        {
            return View;
        }

    }
}