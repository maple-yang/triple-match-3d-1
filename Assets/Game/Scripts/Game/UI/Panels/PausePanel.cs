using System;
using Game.Core.Controllers;
using Game.Data.Models;
using Game.Modules.UIManager;
using Game.Providers.Language;
using Game.Scripts.Game.UI.Widgets;
using Game.UI.Widgets;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI.Panels
{
    public class PausePanel : AnimatedPanel
    {
        [SerializeField] 
        private CustomToggleWidget musicToggleWidget;
        [SerializeField] 
        private CustomToggleWidget soundToggleWidget;
        [SerializeField] 
        private LanguageWidget languageWidget;
        [SerializeField] 
        private Button closeButton;
        [SerializeField] 
        private Button mainMenuButton;

        [Inject] 
        private GameDataModel gameDataModel;
        
        [Inject] 
        private IPauseController pauseController;
        
        [Inject]
        private ILanguageProvider languageProvider;
        
        [Inject]
        private IUIScreenManager uiScreenManager;

        private CompositeDisposable compositeDisposables;

        public ISubject<Unit> EventPausePanelClosed;
        public ISubject<Unit> EventMainMenu;

        public override void Show(bool instant = false, Action onShowed = null)
        {
            base.Show(instant, onShowed);

            EventPausePanelClosed = new Subject<Unit>();
            EventMainMenu = new Subject<Unit>();
            compositeDisposables = new CompositeDisposable();

            musicToggleWidget.Initialized(gameDataModel.IsMusic);
            soundToggleWidget.Initialized(gameDataModel.IsSound);
            languageWidget.Initialized(languageProvider);
            
            musicToggleWidget.EventToggleChanged.Subscribe(EventToggleWidgetMusicChanged).AddTo(compositeDisposables);
            soundToggleWidget.EventToggleChanged.Subscribe(EventToggleWidgetSoundChanged).AddTo(compositeDisposables);
            closeButton.onClick.AddListener(OnCloseClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            
            pauseController.PauseGameOn();

            if (uiScreenManager.IsPanelShowed<MenuPanel>() || gameDataModel.LastOpenedLevelIndex == 0)
            {
                DisableMainMenuButton();
            }
        }

        public override void Hide(bool instant = false, Action onHidden = null)
        {
            musicToggleWidget.DeInitialized();
            soundToggleWidget.DeInitialized();
            languageWidget.DeInitialized();
            compositeDisposables.Dispose();
            compositeDisposables = null;
            closeButton.onClick.RemoveListener(OnCloseClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);

            pauseController.PauseGameOff();

            base.Hide(instant, onHidden);
        }

        private void EventToggleWidgetMusicChanged(BoolReactiveProperty isOn)
        {
            gameDataModel.IsMusic = isOn;
        }
        
        private void EventToggleWidgetSoundChanged(BoolReactiveProperty isOn)
        {
            gameDataModel.IsSound = isOn;
        }

        private void OnCloseClicked()
        {
            EventPausePanelClosed.OnNext(Unit.Default);
        }
        
        private void OnMainMenuClicked()
        {
            EventMainMenu.OnNext(Unit.Default);
        }

        private void DisableMainMenuButton()
        {
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
            mainMenuButton.transform.parent.gameObject.SetActive(false);
            mainMenuButton.interactable = false;
        }
    }
}
