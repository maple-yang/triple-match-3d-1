using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Core.Levels;
using Game.Data;
using Game.Data.Models;
using Game.Levels;
using Game.Modules.UIManager;
using Game.Providers;
using Game.UI.Panels;
using Game.UI.Widgets;
using UniRx;

namespace Game.PlayerLoop
{
    public class PlayerLoopFacade : IPlayerLoopFacade, IPlayerLoopProvider
    {
        public PlayerLoopFacade
        (
            ILevelsProvider levelsProvider,
            ILevelLoader levelLoader,
            IUIScreenManager uiScreenManager,
            GameDataModel gameDataModel
        )
        {
            this.levelsProvider = levelsProvider;
            this.levelLoader = levelLoader;
            this.uiScreenManager = uiScreenManager;
            this.gameDataModel = gameDataModel;
        }

        private readonly ILevelsProvider levelsProvider;
        private readonly ILevelLoader levelLoader;
        private readonly IUIScreenManager uiScreenManager;
        private readonly GameDataModel gameDataModel;

        private CompositeDisposable levelStartCompositeDisposables;
        private CompositeDisposable levelFinishedCompositeDisposables;

        public ISubject<Unit> EventGameLoaded { get; private set; } = new Subject<Unit>();
        public ISubject<Unit> EventLevelStart { get; private set; } = new Subject<Unit>();
        public ISubject<Unit> EventPreLevelLose { get; private set; } = new Subject<Unit>();
        public ISubject<Unit> EventLevelLose { get; private set; } = new Subject<Unit>();
        public ISubject<Unit> EventLevelCompleted { get; private set; } = new Subject<Unit>();
        public ISubject<ELevelResultState> EventLevelContinue { get; private set; } = new Subject<ELevelResultState>();

        public void Initialize()
        {
            levelStartCompositeDisposables = new CompositeDisposable();
            levelFinishedCompositeDisposables = new CompositeDisposable();

            //uiScreenManager.ShowUIPanel<CustomDebugPanel>();
            
            EventGameLoaded.OnNext(Unit.Default);

            if (!gameDataModel.IsFirstStartGame)
            {
                FirstStartGame();
                return;
            }

            EnterMenu();
        }

        private async void EnterMenu()
        {
            var menuPanel = await uiScreenManager.ShowUIPanelAsync<MenuPanel>();
            menuPanel.transform.SetAsFirstSibling();
            menuPanel.EventStartGame.Subscribe(OnStartLevel).AddTo(levelStartCompositeDisposables);
        }

        private void OnStartLevel(LevelData levelData)
        {
            levelStartCompositeDisposables.Clear();
            uiScreenManager.HideUIPanelAsync<MenuPanel>().Forget();
            EventLevelStart.OnNext(Unit.Default);
            StartLevel(levelData);
        }

        private void StartLevel(LevelData levelData)
        {
            levelLoader.LoadLevel(levelData, level =>
            {
                level.EventLevelResult.Subscribe(OnLevelResult).AddTo(levelStartCompositeDisposables);
                levelsProvider.Level = level;
            });
        }

        private async UniTaskVoid CompleteLevel()
        {
            if (gameDataModel.SelectedLevelIndexReactive.Value.Equals(gameDataModel.LastOpenedLevelIndex))
            {
                gameDataModel.LastOpenedLevelIndex++;
                gameDataModel.Save();
            }

            var resultPanel = await uiScreenManager.ShowUIPanelAsync<ResultPanel>();
            resultPanel.EventExitLevel.Subscribe(OnExitLevel).AddTo(levelFinishedCompositeDisposables);
        }

        private async UniTaskVoid FailLevel()
        {
            var resultPanel = await uiScreenManager.ShowUIPanelAsync<ResultPanel>();
            resultPanel.EventExitLevel.Subscribe(OnExitAfterLoseLevel).AddTo(levelFinishedCompositeDisposables);
            resultPanel.EventContinueLevel.Subscribe(OnContinueLevel).AddTo(levelFinishedCompositeDisposables);
            if (resultPanel.CheckLevelExitMenu())
            {
                OnExitAfterLoseLevel(Unit.Default);
            }
        }

        private void OnLevelResult(LevelResultData levelResultData)
        {
            gameDataModel.LevelResultData = levelResultData;

            if (levelResultData.IsWin)
            {
                CompleteLevel().Forget();
                EventLevelCompleted.OnNext(Unit.Default);
            }
            else
            {
                FailLevel().Forget();
                EventPreLevelLose.OnNext(Unit.Default);
            }
        }

        private async void OnExitLevel(Unit unit)
        {
            levelFinishedCompositeDisposables.Clear();
            var task1 = uiScreenManager.HideUIPanelAsync<ResultPanel>();
            await UniTask.WhenAll(task1);
            levelLoader.UnloadCurrentLevel();
            EnterMenu();
        }
        
        private void OnExitAfterLoseLevel(Unit unit)
        {
            EventLevelLose.OnNext(Unit.Default);
            OnExitLevel(unit);
        }
        
        private void OnContinueLevel(ELevelResultState levelResultState)
        {
            levelFinishedCompositeDisposables.Clear();
            uiScreenManager.HideUIPanelAsync<ResultPanel>();
            EventLevelContinue.OnNext(levelResultState);
        }

        private void FirstStartGame()
        {
            EventLevelStart.OnNext(Unit.Default);

            gameDataModel.IsFirstStartGame = true;
            gameDataModel.Save();
            gameDataModel.SelectedLevelIndexReactive.Value = 0;
            gameDataModel.ProgressStarsSelectedLevel.Index = 0;
            gameDataModel.ProgressStarsSelectedLevel.Stars = 0;
            levelsProvider.GetSaveListLevelData(1);
            StartLevel(levelsProvider.GetLevelDataBySelectedLevelNumber());
        }
    }
}