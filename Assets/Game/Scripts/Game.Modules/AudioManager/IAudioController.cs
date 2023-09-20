namespace Game.Modules.AudioManager
{
    public interface IAudioController
    {
        public void TryPlaySound(string audioName);
        public void TryPlayMusic(string audioName);
        public void PauseAudioOn();
        public void PauseAudioOff();
        public void ActiveSound(bool isActive);
        public void ActiveMusic(bool isActive);
    }
}