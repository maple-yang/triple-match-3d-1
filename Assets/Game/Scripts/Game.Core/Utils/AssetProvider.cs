using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Game.Core.Utils
{
    public class AssetProvider : IAssetProvider, IInitializable
    {
        private Dictionary<string, AsyncOperationHandle<GameObject>> loadHandles;

        public void Initialize()
        {
            loadHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        }
        
        public async UniTask<TAsset> TryGetAsset<TAsset>(AssetReference assetReference)
        {
            AsyncOperationHandle<GameObject> handler;
            if (loadHandles.TryGetValue(assetReference.AssetGUID, out var loadHandler))
            {
                handler = loadHandler;
            }
            else
            {
                handler = Addressables.LoadAssetAsync<GameObject>(assetReference);
                loadHandles.Add(assetReference.AssetGUID, handler);
            }

            if (handler.IsValid() && handler.IsDone)
            {
                return handler.Result.GetComponent<TAsset>();
            }

            await handler.ToUniTask();
            return handler.Result.GetComponent<TAsset>();
        }
    }
}