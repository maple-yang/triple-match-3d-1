using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Modules.UIManager
{
    public class LoadPanelCommand : IPanelCommand
    {
        private AsyncOperationHandle<GameObject> loadPanelHandler;
        private CancellationTokenSource cancellationToken; 
        
        public readonly string PanelGuid;
        public bool IsDisposed { get; private set; }

        public LoadPanelCommand(string panelGuid)
        {
            PanelGuid = panelGuid;
            cancellationToken = new CancellationTokenSource();
        }
        
        public async UniTask<TPanel> Execute<TPanel>() where TPanel : UIPanel<UIContext>
        {
            var task = await AssetReferenceExtension.LoadAssetReferenceAsync(new AssetReference(PanelGuid));
            // loadPanelHandler = Addressables.LoadAssetAsync<GameObject>(new AssetReference(PanelGuid));
            // var task = loadPanelHandler.ToUniTask(cancellationToken: cancellationToken.Token);
            return task.GetComponent<TPanel>();
        }
        
        public void Dispose()
        {
            if(IsDisposed) return;
            
            cancellationToken?.Cancel();
            cancellationToken?.Dispose();
            
            if (loadPanelHandler.IsValid())
            {
                Addressables.Release(loadPanelHandler);
            }

            IsDisposed = true;
        }
    }
}