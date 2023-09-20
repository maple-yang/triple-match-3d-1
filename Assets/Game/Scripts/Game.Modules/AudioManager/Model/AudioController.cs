using System.Collections.Generic;

namespace Game.Modules.AudioManager.Model
{
    public class AudioController : IAudioController
    {
        private readonly AudioSourceComponent.Factory audioFactory;
        private readonly AudioParameters audioParameters;
        private AudioSourceComponent musicComponent;
        private List<AudioSourceComponent> soundComponents = new List<AudioSourceComponent>();
        private bool isSound;
        private bool isMusic;
        private bool isPause;

        public AudioController(AudioSourceComponent.Factory audioFactory, AudioParameters audioParameters)
        {
            this.audioFactory = audioFactory;
            this.audioParameters = audioParameters;
        }

        public void TryPlaySound(string audioName)
        {
            if (!isSound)
            {
                return;
            }
            var audioClipParameter = audioParameters.SoundParameters.Find(p => p.AudioName.Equals(audioName));
            if (audioClipParameter == null)
            {
                return;
            }
            var audioSourceComponent = audioFactory.Create(audioClipParameter);
            audioSourceComponent.StopAfterDelay();
            if (!soundComponents.Contains(audioSourceComponent))
            {
                soundComponents.Add(audioSourceComponent);
            }
        }
        
        public void TryPlayMusic(string audioName)
        {
            if (!isMusic || musicComponent != null)
            {
                return;
            }

            var audioClipParameter = audioParameters.MusicParameters.Find(p => p.AudioName.Equals(audioName));
            if (audioClipParameter == null)
            {
                return;
            }
            musicComponent = audioFactory.Create(audioClipParameter);
            if (soundComponents.Contains(musicComponent))
            {
                soundComponents.Remove(musicComponent);
            }
            musicComponent.SetLoop(true);
            if (isPause)
            {
                musicComponent.Pause();
            }
        }

        public void PauseAudioOn()
        {
            isPause = true;
            if (isMusic)
            {
                musicComponent.Pause();
            }

            if (isSound)
            {
                foreach (var soundComponent in soundComponents)
                {
                    if (soundComponent.gameObject.activeSelf)
                    {
                        soundComponent.Pause();
                    }
                }
            }
        }

        public void PauseAudioOff()
        {
            isPause = false;
            if (isMusic)
            {
                musicComponent.Continue();
            }

            if (isSound)
            {
                foreach (var soundComponent in soundComponents)
                {
                    if(soundComponent.gameObject.activeSelf)
                    {
                        soundComponent.Continue();
                    }
                }
            }
        }

        public void ActiveSound(bool isActive)
        {
            isSound = isActive;
        }
        
        public void ActiveMusic(bool isActive)
        {
            isMusic = isActive;
        }
    }
}