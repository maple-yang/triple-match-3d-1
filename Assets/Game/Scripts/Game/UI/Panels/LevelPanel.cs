using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Ads;
using Game.Core.Configurations;
using Game.Core.Controllers;
using Game.Data;
using Game.Data.Models;
using Game.Levels;
using Game.Modules.AudioManager;
using Game.Modules.UIManager;
using Game.Providers;
using Game.Scripts.Game.UI.Widgets;
using Game.Systems.TimeSystem;
using Game.UI.Widgets;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.UI.Panels
{
    public class LevelPanel : AnimatedPanel, IPauseController
    {
        [SerializeField]
        private BoostersWidget boostersWidget;
        
        [SerializeField]
        private GoalWidget goalWidget;
        
        [SerializeField]
        private TimerWidget timerWidget;
        
        [SerializeField]
        private PauseWidget pauseWidget;
        
        [SerializeField]
        private LevelNumberWidget levelNumberWidget;

        [Inject] 
        private ILevelsProvider levelsProvider;
        
        [Inject]
        private ITimer timer;
        
        [Inject]
        private IUIScreenManager uiScreenManager;

        [Inject]
        private IPauseManager pauseManager;
        
        [Inject]
        private IAudioController audioController;
        
        [Inject]
        private IAdsProvider adsProvider;

        [Inject]
        private GameDataModel gameDataModel;
        
        [Inject]
        private BoostersParameters boostersParameters;

        private CompositeDisposable compositeDisposables;

        public GoalWidget GoalWidget => goalWidget;
        public BoostersWidget BoostersWidget => boostersWidget;
        public PauseWidget PauseWidget => pauseWidget;
        
        public ISubject<BoosterType> EventActivateBooster;
        public ISubject<BoosterType> EventBoosterActionStart;
        public ISubject<BoosterType> EventBoosterActionFinished;
        public ISubject<Unit> EventMeinMenu;

        public override void Initialize(UIContext panelContext = default(UIContext))
        {
            base.Initialize(panelContext);

            compositeDisposables = new CompositeDisposable();
            EventActivateBooster = new Subject<BoosterType>();
            EventBoosterActionStart = new Subject<BoosterType>();
            EventBoosterActionFinished = new Subject<BoosterType>();
            EventMeinMenu = new Subject<Unit>();
        }

        public override void Show(bool instant = false, Action onShowed = null)
        {
            base.Show(instant, onShowed);
            
            boostersWidget.Show(gameDataModel.BoostersData.ToArray(), gameDataModel.LastOpenedLevelIndex);
            goalWidget.Show();
            timerWidget.Show(timer);
            pauseWidget.Show();
            levelNumberWidget.Show(gameDataModel.SelectedLevelIndexReactive.Value + 1);
            
            boostersWidget.EventBoosterActivate.Subscribe(OnBoosterClick).AddTo(compositeDisposables);
            boostersWidget.EventExtraBoosterActivate.Subscribe(OnBoosterExtraClick).AddTo(compositeDisposables);
            EventBoosterActionStart.Subscribe(OnBoosterActionStart).AddTo(compositeDisposables);
            EventBoosterActionFinished.Subscribe(OnBoosterActionFinished).AddTo(compositeDisposables);
            pauseWidget.EventShowPausePanel.Subscribe(OnShowPausePanel).AddTo(compositeDisposables);
            gameDataModel.EventAddItemToField.Subscribe(OnAddItemToField).AddTo(compositeDisposables);
            gameDataModel.EventRemoveItemFromField.Subscribe(OnRemoveItemFromField).AddTo(compositeDisposables);
            gameDataModel.EventLevelGoalCompleted.Subscribe(OnSetProgressStars).AddTo(compositeDisposables);

            pauseManager.AddPauseComponent(this);
            
            CustomDebug.EventActionLevel.Subscribe(OnCustomDebugEventActionLevel).AddTo(compositeDisposables);
        }

        public override void Hide(bool instant = false, Action onHidden = null)
        {
            base.Hide(instant, onHidden);
            
            boostersWidget.Hide();
            goalWidget.Hide();
            timerWidget.Hide();
            pauseWidget.Hide();
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            
            compositeDisposables.Dispose();
            compositeDisposables = null;
        }

        private void OnBoosterClick(BoosterType boosterType)
        {
            var data = gameDataModel.BoostersData.SingleOrDefault(t => t.BoosterType == boosterType);
            if (data == null)
            {
                Debug.LogError($"There's no booster data for \'{boosterType}\'!");
                return;
            }

            if (data.Counter.Value > 0)
            {
                EventActivateBooster.OnNext(boosterType);
            }
        }

        private void OnBoosterExtraClick(BoosterType boosterType)
        {
            boostersWidget.SetInteractableButtons(false);
            adsProvider.ShowRewardVideo((succeed) =>
            {
                if (succeed)
                {
                    var data = gameDataModel.GetBoosterData(boosterType);
                    data.Counter.Value += boostersParameters.AddBoosterForAds;
                }
                boostersWidget.SetInteractableButtons(true);
            });
        }
        
        private void OnBoosterActionStart(BoosterType boosterType)
        {
            boostersWidget.ShowBoosterEffect(boosterType);
            boostersWidget.SetInteractableButtons(false);
        }

        private void OnBoosterActionFinished(BoosterType boosterType)
        {
            boostersWidget.HideBoosterEffect(boosterType);
            boostersWidget.SetInteractableButtons(true);
        }

        public void DisableAllBoosters()
        {
            boostersWidget.SetInteractableButtons(false);
            foreach (var booster in boostersWidget.BoosterView)
            {
                boostersWidget.HideBoosterEffect(booster.BoosterType);
            }
        }
        
        private void OnSetProgressStars(Unit unit)
        {
            var selectedLevelIndex = gameDataModel.SelectedLevelIndexReactive.Value;
            var levelData = levelsProvider.GetLevelDataBySelectedLevelNumber();
            var percentSuccess = timer.Time.Value.TotalSeconds * 100 / levelData.LevelSettings.Timer.TotalSeconds;
            var amountStars = 0;
            for (int i = 0; i < levelData.LevelSettings.ProgressStarts.Length; i++)
            {
                var progressStart = levelData.LevelSettings.ProgressStarts[i];
                if (progressStart.PercentTimeReceive < percentSuccess)
                {
                    amountStars = progressStart.NumberStar;
                }
            }
            gameDataModel.SetNumberLevelProgressStars(selectedLevelIndex, amountStars);
        }
        
        private void OnAddItemToField(string itemId)
        {
            goalWidget.AddItemGoal(itemId);
        }
        
        private void OnRemoveItemFromField(string itemId)
        {
            goalWidget.RemoveItemGoal(itemId);
        }
        
        private void OnShowPausePanel(Unit unit)
        {
            ShowPausePanel();
            boostersWidget.SetInteractableButtons(false);
            pauseWidget.SetInteractableButton(false);
            audioController.TryPlaySound(AudioNameData.CLICK_SOUND);
        }
        
        private void ShowPausePanel()
        {
            uiScreenManager.ShowUIPanel<PausePanel>(panelShowed: (pausePanel) =>
            {
                pausePanel.EventPausePanelClosed.Subscribe(OnPausePanelClosed).AddTo(compositeDisposables);
                pausePanel.EventMainMenu.Subscribe(OnMainMenu).AddTo(compositeDisposables);
            });
        }

        private void OnPausePanelClosed(Unit unit)
        {
            uiScreenManager.HideUIPanel<PausePanel>();
            boostersWidget.SetInteractableButtons(true);
            pauseWidget.SetInteractableButton(true);
            audioController.TryPlaySound(AudioNameData.CLICK_SOUND);
        }
        
        private void OnMainMenu(Unit unit)
        {
            uiScreenManager.HideUIPanel<PausePanel>();
            EventMeinMenu.OnNext(unit);
        }

        public void PauseGameOn()
        {
            timer.Pause();
        }

        public void PauseGameOff()
        {
            timer.Continue();
        }

        private void OnCustomDebugEventActionLevel(EDebugLevelResultState levelResultState)
        {
            if (levelResultState == EDebugLevelResultState.None)
            {
                OnSetProgressStars(Unit.Default);
            }
        }
    }
}