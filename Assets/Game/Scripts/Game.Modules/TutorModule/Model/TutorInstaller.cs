using UnityEngine;
using Zenject;

namespace Game.Modules.TutorModule.Model
{
    public class TutorInstaller: MonoInstaller<TutorInstaller>
    {
        [SerializeField]
        private TutorProvider tutorProvider;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<TutorProvider>().FromInstance(tutorProvider).AsSingle();
            Container.BindInterfacesTo<TutorController>().AsSingle();
        }
    }
}