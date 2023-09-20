using System;
using Cysharp.Threading.Tasks;
using Game.Data.Models;
using Game.Levels;
using Game.Managers;
using Game.Modules.TutorModule;
using Game.Tutor.Data;
using UniRx;
using Zenject;

namespace Game.Tutor
{
    public class GameTutorManager : IGameTutorManager, IDisposable
    {
        public GameTutorManager(
            GameDataModel gameDataModel,
            ITutorController tutorController,
            CameraManager cameraManager)
        {
            this.gameDataModel = gameDataModel;
            this.tutorController = tutorController;
            this.cameraManager = cameraManager;
        }

        private readonly GameDataModel gameDataModel;
        private readonly CameraManager cameraManager;
        private readonly ITutorController tutorController;
        private CompositeDisposable compositeDisposable;

        [Inject]
        private void Initialization()
        {
            compositeDisposable = new CompositeDisposable();
            tutorController.EventTutorStepShow.Subscribe(OnTutorStepShow).AddTo(compositeDisposable);
            tutorController.EventTutorStepHide.Subscribe(OnTutorStepHide).AddTo(compositeDisposable);
        }
        
        public void Dispose()
        {
            compositeDisposable.Dispose();
        }

        public async UniTask TryShowTutorAsync(GameLevel gameLevel)
        {
            var tutorLogicData = new GameTutorLogicData
            {
                ParentTransform = gameLevel.LevelPanel.transform.parent,
                GameDataModel = gameDataModel,
                GameLevel = gameLevel,
                CameraManager = cameraManager
            };
            var tutorLogic = tutorController.CheckAvailabilityTutor(tutorLogicData);
            if (tutorLogic == null)
            {
                return;
            }
            await tutorController.ShowTutor(tutorLogic, tutorLogicData);
        }

        private void OnTutorStepShow(ITutorLogic tutorLogic)
        {
            
        }
        
        private void OnTutorStepHide(ITutorLogic tutorLogic)
        {
            var tutorLogicCache = tutorLogic.ToString();
            if (gameDataModel.CompletedTutorCache.Contains(tutorLogicCache))
            {
                return;
            }
            gameDataModel.CompletedTutorCache.Add(tutorLogicCache);
            gameDataModel.Save();
        }
    }
}