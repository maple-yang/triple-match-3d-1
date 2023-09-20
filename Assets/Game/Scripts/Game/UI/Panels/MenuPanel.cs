using DG.Tweening;
using Game.Ads;
using Game.Core.Controllers;
using Game.Core.Levels;
using Game.Data.Models;
using Game.Modules.AudioManager;
using Game.Modules.UIManager;
using Game.Providers;
using Game.Scripts.Game.UI.Widgets;
using Game.UI.Widgets;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI.Panels
{
    public class MenuPanel : AnimatedPanel
    {
        [SerializeField]
        private PauseWidget pauseWidget;
        
        [SerializeField]
        private Button startGameButton;
        
        [SerializeField]
        private CanvasGroup startGameCanvasGroup;
        
        [Inject]
        private LivesWidget livesWidget;
        
        [Inject]
        private ILevelMapProvider levelMapProvider;

        [Inject]
        private GameDataModel gameDataModel;

        [Inject]
        private IAudioController audioController;
        
        [Inject]
        private IAdsProvider adsProvider;
        
        [Inject]
        private IUIScreenManager uiScreenManager;

        private CompositeDisposable compositeDisposable;
        private Sequence sequence;

        public ISubject<LevelData> EventStartGame { get; private set; }

        public override void Initialize(UIContext panelContext = default(UIContext))
        {
            base.Initialize(panelContext);
            
            EventStartGame = new Subject<LevelData>();
            compositeDisposable = new CompositeDisposable();
            startGameButton.interactable = false;
            
            var isShowInter = adsProvider.ShowInterstitial();

            levelMapProvider.Initialization();
            levelMapProvider.EventCurveMapActive.Subscribe(OnEventLevelMapActive).AddTo(compositeDisposable);
            levelMapProvider.Show(isShowInter);
            livesWidget.Show();
            pauseWidget.Show();
            pauseWidget.EventShowPausePanel.Subscribe(OnShowPausePanel).AddTo(compositeDisposable);

            startGameButton.onClick.AddListener(OnStartGameButtonClick);
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            
            startGameButton.onClick.RemoveListener(OnStartGameButtonClick);
            compositeDisposable.Dispose();
            compositeDisposable.Clear();
            TryKillSequence();
            levelMapProvider.Hide();
            livesWidget.Hide();
            pauseWidget.Hide();
        }

        private void OnStartGameButtonClick()
        {
            audioController.TryPlaySound(AudioNameData.CLICK_SOUND);

            if (gameDataModel.LivesCount.Value <= 0)
            {
                livesWidget.ShowAddLivesPopup();
                return;
            }
            
            startGameButton.interactable = false;
            EventStartGame.OnNext(levelMapProvider.GetSelectedLevelData());
        }

        private void OnEventLevelMapActive(bool isActive)
        {
            startGameButton.interactable = !isActive;

            TryKillSequence();
            sequence = DOTween.Sequence();
            sequence.Append(startGameCanvasGroup.DOFade(isActive ? 0 : 1, isActive ? 0.1f : 0.5f)
                .SetDelay(isActive ? 0 : 0.5f));
        }
        
        private void OnShowPausePanel(Unit unit)
        {
            ShowPausePanel();
            pauseWidget.SetInteractableButton(false);
            audioController.TryPlaySound(AudioNameData.CLICK_SOUND);
        }
        
        private void ShowPausePanel()
        {
            uiScreenManager.ShowUIPanel<PausePanel>(panelShowed: (pausePanel) =>
            {
                pausePanel.EventPausePanelClosed.Subscribe(OnPausePanelClosed).AddTo(compositeDisposable);
            });
        }
        
        private void OnPausePanelClosed(Unit unit)
        {
            uiScreenManager.HideUIPanel<PausePanel>();
            pauseWidget.SetInteractableButton(true);
            audioController.TryPlaySound(AudioNameData.CLICK_SOUND);
        }
        
        private void TryKillSequence()
        {
            if (sequence != null)
            {
                sequence.Kill();
                sequence = null;
            }
        }
        
        private void OnDestroy()
        {
            TryKillSequence();
        }
    }
}