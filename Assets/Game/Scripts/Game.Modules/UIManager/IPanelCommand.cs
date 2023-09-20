using System;
using Cysharp.Threading.Tasks;

namespace Game.Modules.UIManager
{
    public interface IPanelCommand : IDisposable
    {
        UniTask<TPanel> Execute<TPanel>() where TPanel : UIPanel<UIContext>;
    }
}