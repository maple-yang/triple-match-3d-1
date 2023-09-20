using Game.Providers.Goal;
using Zenject;

namespace Game.Installers
{
    public class UILevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GoalProvider>().AsSingle();
        }
    }
}