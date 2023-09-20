using System;
using Game.Data.Models;
using Game.Levels;
using Game.Modules.AudioManager;
using Game.Providers;
using UniRx;
using Zenject;

namespace Game.Core.Controllers
{
    public class AudioGameManager : IPauseController, IDisposable
    {
        private readonly GameDataModel gameDataModel;
        private readonly IAudioController audioController;
        private readonly IPauseManager pauseManager;
        private readonly IPlayerLoopProvider playerLoopProvider;
        private CompositeDisposable compositeDisposable; 
        
        public AudioGameManager(
            GameDataModel gameDataModel,
            IPauseManager pauseManager,
            IAudioController audioController,
            IPlayerLoopProvider playerLoopProvider)
        {
            this.gameDataModel = gameDataModel;
            this.audioController = audioController;
            this.pauseManager = pauseManager;
            this.playerLoopProvider = playerLoopProvider;
        }
        
        [Inject]
        private void Initialization()
        {
            compositeDisposable = new CompositeDisposable();
            playerLoopProvider.EventGameLoaded.Subscribe(OnGameLoaded).AddTo(compositeDisposable);
        }

        private void OnGameLoaded(Unit unit)
        {
            pauseManager.AddPauseComponent(this);
            
            gameDataModel.IsSoundChange.Subscribe(ActiveSound).AddTo(compositeDisposable);
            gameDataModel.IsMusicChange.Subscribe(ActiveMusic).AddTo(compositeDisposable);
            ActiveSound(gameDataModel.IsSound.Value);
            ActiveMusic(gameDataModel.IsMusic.Value);
            
            playerLoopProvider.EventLevelStart.Subscribe(OnLevelStart).AddTo(compositeDisposable);
            playerLoopProvider.EventLevelCompleted.Subscribe(OnLevelCompleted).AddTo(compositeDisposable);
            playerLoopProvider.EventPreLevelLose.Subscribe(OnPreLevelLose).AddTo(compositeDisposable);
        }

        private void ActiveSound(bool isActive)
        {
            audioController.ActiveSound(isActive);
        }
        
        private void ActiveMusic(bool isActive)
        {
            audioController.ActiveMusic(isActive);
            audioController.TryPlayMusic(AudioNameData.MAIN_MUSIC);
        }
        
        public void PauseGameOn()
        {
            audioController.PauseAudioOn();
        }

        public void PauseGameOff()
        {
            audioController.PauseAudioOff();
        }
        
        private void OnLevelStart(Unit unit)
        {
        }

        private void OnLevelCompleted(Unit unit)
        {
            audioController.TryPlaySound(AudioNameData.LEVEL_COMPLETED_SOUND);
        }
        
        private void OnPreLevelLose(Unit unit)
        {
            audioController.TryPlaySound(AudioNameData.LEVEL_LOSE_SOUND);
        }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }
    }
}