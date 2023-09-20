using Zenject;
using Game.Systems.TimeSystem;

namespace Game.Systems.Installers
{
    public sealed class SystemsInstaller : Installer<SystemsInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameTimer>().AsSingle();
        }
    }
}