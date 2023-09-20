using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Core.Configurations
{
    [CreateAssetMenu(fileName = nameof(CoreLoaderConfigs), menuName = "Configs/GameCore/"+nameof(CoreLoaderConfigs), order = 0)]
    public class CoreLoaderConfigs : ScriptableObjectInstaller<CoreLoaderConfigs>
    {
        [SerializeField]
        private List<MonoInstaller> prefabInstallers = new List<MonoInstaller>();
        
        [SerializeField]
        private List<ScriptableObject> configs = new List<ScriptableObject>();
        
        public List<MonoInstaller> PrefabInstallers => prefabInstallers;

        public static CoreLoaderConfigs Get()
        {
            return Resources.Load<CoreLoaderConfigs>(nameof(CoreLoaderConfigs));
        }

        public override void InstallBindings()
        {
            Container.BindInstance(this);
            
            foreach (var config in configs)
            {
                Container.BindInterfacesAndSelfTo(config.GetType()).FromInstance(config);
                Container.QueueForInject(config);
            }
        }
    }
}