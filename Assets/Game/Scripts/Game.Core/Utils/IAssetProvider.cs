using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Game.Core.Utils
{
    public interface IAssetProvider
    {
        UniTask<TAsset> TryGetAsset<TAsset>(AssetReference assetReference);
    }
}