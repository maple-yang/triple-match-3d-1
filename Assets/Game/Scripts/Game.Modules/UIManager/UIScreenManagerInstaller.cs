using UnityEngine;
using Zenject;

namespace Game.Modules.UIManager
{
    public class UIScreenManagerInstaller : MonoInstaller<UIScreenManagerInstaller>
    {
        [SerializeField]
        private UIScreenManager uiScreenManagerPrefab;

        [SerializeField]
        private UIPanels uiPanels;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<UIScreenManager>().FromComponentInNewPrefab(uiScreenManagerPrefab).AsSingle();
            Container.BindInstance(uiPanels).AsSingle();
        }
    }
}
