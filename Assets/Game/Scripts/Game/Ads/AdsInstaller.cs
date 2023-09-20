using UnityEngine;
using Zenject;

namespace Game.Ads
{
    public class AdsInstaller : MonoInstaller<AdsInstaller>
    {
        [SerializeField] 
        private AdsMessageController adsMessageController;
        
        public override void InstallBindings()
        {
            Container.Bind<AdsMessageController>().FromInstance(adsMessageController).AsSingle();
            Container.BindInterfacesTo<AdsProvider>().AsSingle();
        }
    }
}