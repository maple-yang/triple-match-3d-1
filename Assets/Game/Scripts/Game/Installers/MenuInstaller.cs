using Game.Core.Controllers;
using Game.Modules.CurveMapModule;
using Game.Modules.CurveMapModule.Model;
using Game.Scripts.Game.UI.Widgets;
using Game.UI.Widgets;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class MenuInstaller : MonoInstaller
    {
        [SerializeField]
        private bool useCurveMap;
        
        [SerializeField, HideIf("useCurveMap", true)]
        private LevelMapWidget levelMapWidget;
        
        [SerializeField, ShowIf("useCurveMap", true)]
        private CurveLevelMapWidget curveLevelMapWidget;

        [SerializeField]
        private LevelMapProgressAnimator levelMapProgressAnimator;
        
        [SerializeField]
        private LivesWidget livesWidget;
        
        public override void InstallBindings()
        {
            Container.Bind<LivesWidget>().FromInstance(livesWidget).AsSingle();
            if (useCurveMap)
            {
                Container.BindInterfacesTo<CurveLevelMapWidget>().FromInstance(curveLevelMapWidget).AsSingle();
                Container.BindInterfacesTo<CurveMapObserver>().AsSingle();
            }
            else
            {
                Container.BindInterfacesTo<LevelMapWidget>().FromInstance(levelMapWidget).AsSingle();
                Container.BindInterfacesTo<LevelMapObserver>().AsSingle();
            }
            Container.Bind<LevelMapProgressAnimator>().FromInstance(levelMapProgressAnimator).AsSingle();
        }
    }
}