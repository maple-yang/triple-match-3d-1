using System.Linq;
using Game.Core.Configurations;
using Game.Core.Controllers;
using Game.Core.Utils;
using Game.Data.Installers;
using Game.Providers;
using Game.Providers.Language;
using Game.Systems.Installers;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class GameInstaller : Installer<GameInstaller>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ProjectContextRegistration()
        {
            ProjectContext.PreInstall += OnPreInstallProjectContext;
        }

        private static void OnPreInstallProjectContext()
        {
            var normInstallers = ProjectContext.Instance.NormalInstallers.ToList();
            normInstallers.Insert(0, new GameInstaller());
            ProjectContext.Instance.NormalInstallers = normInstallers;
            
            // Add SO installers
            var scInstallers = ProjectContext.Instance.ScriptableObjectInstallers.ToList();
            var coreLoaderConfig = CoreLoaderConfigs.Get();
            scInstallers.Add(coreLoaderConfig);
            ProjectContext.Instance.ScriptableObjectInstallers = scInstallers;

            // Add prefabs installers
            var prefabInstallers = ProjectContext.Instance.InstallerPrefabs.ToList();
            prefabInstallers.AddRange(coreLoaderConfig.PrefabInstallers);
            ProjectContext.Instance.InstallerPrefabs = prefabInstallers;
        }

        public override void InstallBindings()
        {
            // Install features
            
            DataInstaller.Install(Container);
            SystemsInstaller.Install(Container);
            
            Container.Bind<ILevelsProvider>().To<PlayerLevelsProvider>().AsSingle();
            Container.BindInterfacesTo<AssetProvider>().AsSingle();
            Container.BindInterfacesTo<PauseController>().AsSingle();
            Container.BindInterfacesTo<LanguageProvider>().AsSingle();
        }
    }
}