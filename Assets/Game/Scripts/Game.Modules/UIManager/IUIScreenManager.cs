using System;
using Cysharp.Threading.Tasks;

namespace Game.Modules.UIManager
{
    public interface IUIScreenManager
    {
        bool IsPanelShowed<TPanel>() where TPanel : UIPanel<UIContext>;

        void ShowUIPanel<TPanel>(UIContext context = null, bool instant = false, Action<TPanel> panelShowed = null)
            where TPanel : UIPanel<UIContext>;

        UniTask<TPanel> ShowUIPanelAsync<TPanel>(UIContext context = null, bool instant = false) where TPanel : UIPanel<UIContext>;

        void UpdateUIPanel<TPanel>(UIContext context);
        
        void HideUIPanel<TPanel>(bool instant = false, Action panelHidden = null) where TPanel : UIPanel<UIContext>;
        
        UniTask HideUIPanelAsync<TPanel>(bool instant = false) where TPanel : UIPanel<UIContext>;

        bool TryGetUIPanel<TPanel>(out TPanel panel) where TPanel : UIPanel<UIContext>;
    }
}