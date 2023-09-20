using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Ads;
using Game.Core.Configurations;
using Game.Data;
using Game.Data.Models;
using Game.Levels;
using Game.Modules.AudioManager;
using Game.Modules.UIManager;
using Game.Providers;
using Game.Systems.TimeSystem;
using Game.UI.Widgets;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.UI.Panels
{
    public class ResultPanel : AnimatedPanel
    {
        [SerializeField]
        private CompleteLevelWidget completeLevelWidget;
        
        [SerializeField]
        private LoseLevelWidget loseLevelWidget;
        
        [Inject]
        private GameDataModel gameDataModel;

        [Inject] 
        private ITimer timer;
        
        [Inject] 
        private IAdsProvider adsProvider;
        
        [Inject] 
        private IAudioController audioController;
        
        [Inject] 
        private ResultPanelParameters resultPanelParameters;

        private CompositeDisposable compositeDisposable;

        public ISubject<Unit> EventExitLevel;
        public ISubject<ELevelResultState> EventContinueLevel;

        public override void Show(bool instant = false, Action onShowed = null)
        {
            base.Show(instant, onShowed);
            
            EventExitLevel = new Subject<Unit>();
            EventContinueLevel = new Subject<ELevelResultState>();
            compositeDisposable = new CompositeDisposable();
            var gameLevelResultData = (GameLevelResultData)gameDataModel.LevelResultData;

            if (gameLevelResultData.IsWin)
            {
                loseLevelWidget.Hide();
                completeLevelWidget.Show(gameDataModel, resultPanelParameters, timer);
                completeLevelWidget.EventExitButton.Subscribe(OnExitButtonClick).AddTo(compositeDisposable);
            }
            else
            {
                completeLevelWidget.Hide();
                loseLevelWidget.Show(gameLevelResultData.LevelResultState, resultPanelParameters);
                loseLevelWidget.EventExitButton.Subscribe(OnExitButtonClick).AddTo(compositeDisposable);
                loseLevelWidget.EventContinueButton.Subscribe(OnContinueButtonClick).AddTo(compositeDisposable);
            }
        }

        public override void Hide(bool instant = false, Action onHidden = null)
        {
            compositeDisposable.Dispose();
            completeLevelWidget.Hide();
            loseLevelWidget.Hide();

            base.Hide(instant, onHidden);
        }

        public bool CheckLevelExitMenu()
        {
            var gameLevelResultData = (GameLevelResultData)gameDataModel.LevelResultData;
            return gameLevelResultData.LevelResultState == ELevelResultState.ExitMenu;
        }

        private void OnExitButtonClick(Unit unit)
        {
            audioController.TryPlaySound(AudioNameData.CLICK_SOUND);
            EventExitLevel.OnNext(Unit.Default);
        }

        private void OnContinueButtonClick(ELevelResultState levelResultState)
        {
            audioController.TryPlaySound(AudioNameData.CLICK_SOUND);
            adsProvider.ShowRewardVideo((succeed) =>
            {
                if (succeed)
                {
                    EventContinueLevel.OnNext(levelResultState);
                }
                else
                {
                    loseLevelWidget.SetInteractableButtons(true);
                }
            });
        }
    }
}