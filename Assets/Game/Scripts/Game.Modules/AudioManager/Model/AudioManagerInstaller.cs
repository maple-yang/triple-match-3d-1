using UnityEngine;
using Zenject;

namespace Game.Modules.AudioManager.Model
{
    public class AudioManagerInstaller: MonoInstaller<AudioManagerInstaller>
    {
        [SerializeField]
        private AudioParameters audioParameters;
        
        public override void InstallBindings()
        {
            Container.Bind<AudioParameters>().FromInstance(audioParameters).AsSingle();
            Container.Bind<IAudioController>().To<AudioController>().AsSingle();
            Container.BindFactory<AudioClipParameter, AudioSourceComponent, AudioSourceComponent.Factory>()
                .FromPoolableMemoryPool<AudioClipParameter, AudioSourceComponent, AudioSourceComponentPool>(poolBinder => poolBinder
                    .WithInitialSize(5)
                    .FromComponentInNewPrefab(audioParameters.AudioSourceComponentReference)
                    .UnderTransform(transform));
        }
        
        private class AudioSourceComponentPool : MonoPoolableMemoryPool<AudioClipParameter, IMemoryPool, AudioSourceComponent>
        {
        }
    }
}