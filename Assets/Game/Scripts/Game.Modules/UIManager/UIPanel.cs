using System;
using UnityEngine;

namespace Game.Modules.UIManager
{
    public abstract class UIPanel : UIPanel<UIContext>
    {
    }

    public abstract class UIPanel<TPanelContext> : MonoBehaviour where TPanelContext : UIContext
    {
        public abstract bool IsActive { get; }
        public abstract string LayerID { get; }

        public abstract void Initialize(TPanelContext panelContext = null);
        public abstract void DeInitialize();
        public abstract void Show(bool instant = false, Action onShowed = null);
        public abstract void UpdatePanel(TPanelContext newPanelContext = null);
        public abstract void Hide(bool instant = false, Action onHidden = null);
    }
}