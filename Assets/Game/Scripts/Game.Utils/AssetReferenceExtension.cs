using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Utils
{
    public static class AssetReferenceExtension
    {
        private static Dictionary<int, Action<GameObject>> m_eventLoadedDictionary = new Dictionary<int, Action<GameObject>>();
        
        public static async Task<GameObject> LoadAssetReferenceAsync(AssetReference assetReference)
        {
            if (assetReference.RuntimeKeyIsValid())
            {
                if (assetReference.IsValid())
                {
                    if (assetReference.Asset)
                    {
                        return (GameObject)assetReference.Asset;
                    }
                    else
                    {
                        return await TryLoadAssetAsync(assetReference);
                    }
                }
                else
                {
                    return await TryLoadAssetAsync(assetReference);
                }
            }

            return null;
        }

        public static void LoadAssetReferenceAction(AssetReference assetReference, Action<GameObject> eventLoaded)
        {
            if (assetReference.RuntimeKeyIsValid())
            {
                if (assetReference.IsValid())
                {
                    if (assetReference.Asset)
                    {
                        eventLoaded?.Invoke((GameObject)assetReference.Asset);
                    }
                    else
                    {
                        TryLoadAsset(assetReference, eventLoaded);
                    }
                }
                else
                {
                    TryLoadAsset(assetReference, eventLoaded);
                }
            }
        }

        private static async Task<GameObject> TryLoadAssetAsync(AssetReference assetReference)
        {
            AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(assetReference);

            await asyncOperationHandle.Task;

            return asyncOperationHandle.Result;
        }

        private static void TryLoadAsset(AssetReference assetReference, Action<GameObject> eventLoaded)
        {
            AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(assetReference);
            asyncOperationHandle.Completed += OnLoadCompleted;

            int asyncOperationHandleHashCode = asyncOperationHandle.GetHashCode();

            if (!m_eventLoadedDictionary.ContainsKey(asyncOperationHandleHashCode))
                m_eventLoadedDictionary[asyncOperationHandleHashCode] = new Action<GameObject>(eventLoaded);
            else
                m_eventLoadedDictionary[asyncOperationHandleHashCode] += eventLoaded;
        }

        private static void OnLoadCompleted(AsyncOperationHandle<GameObject> asyncOperationHandle)
        {
            int asyncOperationHandleHashCode = asyncOperationHandle.GetHashCode();

            if (!m_eventLoadedDictionary.ContainsKey(asyncOperationHandleHashCode))
                return;

            for (int i = 0, l = m_eventLoadedDictionary[asyncOperationHandleHashCode].GetInvocationList().Length; i < l; i++)
            {
                asyncOperationHandle.Completed -= OnLoadCompleted;
            }

            Action<GameObject> callbacks = m_eventLoadedDictionary[asyncOperationHandleHashCode];
            m_eventLoadedDictionary.Remove(asyncOperationHandleHashCode);
            callbacks?.Invoke(asyncOperationHandle.Result);
        }
    }
}