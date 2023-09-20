using UnityEngine;
using Zenject;

namespace Game.Modules.LivesManager.Model
{
    public class LivesManagerInstaller : MonoInstaller<LivesManagerInstaller>
    {
        [SerializeField]
        private LivesParameters livesParameters;
        
        public override void InstallBindings()
        {
            Container.Bind<LivesParameters>().FromInstance(livesParameters).AsSingle();
            Container.BindInterfacesTo<LivesManager>().AsSingle();
        }
    }
}