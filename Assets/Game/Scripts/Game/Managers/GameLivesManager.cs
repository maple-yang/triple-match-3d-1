using System;
using Game.Data.Models;
using Game.Modules.LivesManager;
using Game.Modules.LivesManager.Model;
using Game.Providers;
using UniRx;
using Zenject;

namespace Game.Managers
{
    public class GameLivesManager : IDisposable
    {
        private readonly GameDataModel gameDataModel;
        private readonly ILivesController livesController;
        private readonly ILivesProvider livesProvider;
        private readonly IPlayerLoopProvider playerLoopProvider;
        private CompositeDisposable compositeDisposable;

        public GameLivesManager(
            GameDataModel gameDataModel,
            ILivesController livesController,
            ILivesProvider livesProvider,
            IPlayerLoopProvider playerLoopProvider)
        {
            this.gameDataModel = gameDataModel;
            this.livesController = livesController;
            this.livesProvider = livesProvider;
            this.playerLoopProvider = playerLoopProvider;
        }
        
        [Inject]
        private void Initialization()
        {
            compositeDisposable = new CompositeDisposable();
            var livesData = new LivesData
            {
                LivesCount = gameDataModel.LivesCount.Value,
                TimestampRechargeLives = gameDataModel.TimestampRechargeLives,
                TemporaryLive = gameDataModel.TemporaryLive
            };
            livesProvider.Initialization(livesData);
            livesProvider.EventLivesDataUpdated.Subscribe(OnLivesDataUpdated).AddTo(compositeDisposable);
            livesProvider.EventTemporaryLiveAction.Subscribe(OnTemporaryLiveAction).AddTo(compositeDisposable);
            playerLoopProvider.EventLevelStart.Subscribe(OnLevelStart).AddTo(compositeDisposable);
            playerLoopProvider.EventLevelLose.Subscribe(OnLevelLose).AddTo(compositeDisposable);
            playerLoopProvider.EventLevelCompleted.Subscribe(OnLevelCompleted).AddTo(compositeDisposable);
        }

        private void OnLivesDataUpdated(LivesData livesData)
        {
            gameDataModel.LivesCount.Value = livesData.LivesCount;
            gameDataModel.TimestampRechargeLives = livesData.TimestampRechargeLives;
            gameDataModel.Save();
        }

        private void OnTemporaryLiveAction(int temporaryLive)
        {
            gameDataModel.TemporaryLive = temporaryLive;
            gameDataModel.Save();
        }

        private void OnLevelStart(Unit unit)
        {
            livesController.EventLevelStart.OnNext(unit);
        }
        
        private void OnLevelLose(Unit unit)
        {
            livesController.EventLevelLose.OnNext(unit);
        }
        
        private void OnLevelCompleted(Unit unit)
        {
            livesController.EventLevelCompleted.OnNext(unit);
        }
        
        public void Dispose()
        {
            //compositeDisposable?.Dispose();
        }
    }
}