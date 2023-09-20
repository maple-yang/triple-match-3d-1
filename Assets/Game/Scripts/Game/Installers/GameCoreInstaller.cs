using Game.Core.Components;
using Game.Core.Controllers;
using Game.Core.Levels;
using Game.Managers;
using Game.PlayerLoop;
using Game.Systems.PlayerInput;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class GameCoreInstaller : MonoInstaller<GameCoreInstaller>
    {
        [SerializeField]
        private GameCoreInitializer gameCoreInitializer;

        [SerializeField]
        private LevelLoader levelLoader;
        
        [SerializeField]
        private CameraManager cameraManager;
        
        public override void InstallBindings()
        {
            Container.Bind<CameraManager>().FromInstance(cameraManager).AsSingle();
            Container.BindInterfacesAndSelfTo<GameCoreInitializer>().FromInstance(gameCoreInitializer).AsSingle();
            Container.Bind<ILevelLoader>().To<LevelLoader>().FromInstance(levelLoader).AsSingle();
            
            Container.BindInterfacesTo<PlayerLoopFacade>().AsSingle();
            //Container.BindInterfacesTo<GameInput>().AsSingle();

            Container.Bind<ItemView.Factory>().FromNew().AsSingle();
            
            Container.BindInterfacesTo<AudioGameManager>().AsSingle();
            Container.BindInterfacesTo<GameLivesManager>().AsSingle();
            Container.BindInterfacesTo<BoosterManager>().AsSingle();
            Container.BindInterfacesTo<LanguageManager>().AsSingle();
        }
    }
}