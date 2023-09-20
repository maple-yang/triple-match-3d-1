using Game.Core.Components;
using Game.Core.Controllers;
using Game.Tutor;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class LevelInstaller: MonoInstaller
    {
        [SerializeField]
        private ItemsContainer itemsContainer;

        [SerializeField]
        private ItemsInventory itemsInventory;
        
        public override void InstallBindings()
        {
            Container.Bind<ItemsContainer>().FromInstance(itemsContainer).AsSingle();
            Container.Bind<ItemsInventory>().FromInstance(itemsInventory).AsSingle();
            Container.BindInterfacesTo<BoosterController>().AsSingle();
            Container.BindInterfacesTo<GameEffectController>().AsSingle();
            Container.BindInterfacesTo<GameTutorManager>().AsSingle();
        }
    }
}