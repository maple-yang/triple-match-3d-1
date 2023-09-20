using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Game.Modules.AudioManager.Model
{
    public class AudioSourceComponent : MonoBehaviour, IPoolable<AudioClipParameter, IMemoryPool>
    {
        [SerializeField] 
        private AudioSource audioSource;
        
        private IMemoryPool pool;

        public void OnSpawned(AudioClipParameter audioClipParameter, IMemoryPool memoryPool)
        {
            pool = memoryPool;
            audioSource.clip = audioClipParameter.AudioClip;
            audioSource.volume = audioClipParameter.Volume;
            audioSource.Play();
        }
        
        public void OnDespawned()
        {
            pool = null;
        }

        public void StopAfterDelay()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetDelay(audioSource.clip.length).OnKill(() =>
            {
                pool.Despawn(this);
            });
        }

        public void Pause()
        {
            if (audioSource.clip == null || audioSource == null)
            {
                return;
            }
            audioSource.Pause();
        }

        public void Continue()
        {
            if (audioSource.clip == null || audioSource == null)
            {
                return;
            }
            audioSource.Play();
        }

        public void SetLoop(bool isLoop)
        {
            audioSource.loop = isLoop;
        }

        public class Factory : PlaceholderFactory<AudioClipParameter, AudioSourceComponent>
        {
        }
    }
}