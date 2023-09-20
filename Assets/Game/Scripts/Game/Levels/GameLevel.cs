using System;
using Game.Core.Components;
using Game.Core.Configurations;
using Game.Core.Controllers;
using Game.Core.Levels;
using Game.Data;
using Game.Data.Models;
using Game.Managers;
using Game.Modules.AudioManager;
using Game.Modules.UIManager;
using Game.Providers;
using Game.Systems.TimeSystem;
using Game.Tutor;
using Game.UI.Panels;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Levels
{
    public class GameLevel : Level
    {
        [Inject]
        private ItemsContainer itemsContainer;

        [Inject]
        private ItemsInventory itemsInventory;

        [Inject]
        private ItemView.Factory itemsFactory;
        
        [Inject]
        private ItemsParameters itemsParameters;
        
        [Inject] 
        private ResultPanelParameters resultPanelParameters;

        [Inject]
        private ITimer timer;

        [Inject]
        private IUIScreenManager uiScreenManager;
        
        [Inject]
        private IBoostersController boostersController;
        
        [Inject]
        private IGameTutorManager gameTutorManager;
        
        [Inject]
        private IPlayerLoopProvider playerLoopProvider;
        
        [Inject]
        private IAudioController audioController;
        
        [Inject]
        private GameDataModel gameDataModel;
        
        [Inject]
        private CameraManager cameraManager;
        
        private ILevelCreator levelCreator;
        private ItemsController itemsController;
        private LevelSettings settings;
        private GameLevelResultData gameLevelResultData;

        private CompositeDisposable inLevelDisposables;
        private CompositeDisposable compositeDisposables;
        
        public LevelPanel LevelPanel { get; set; }
        public ItemsContainer ItemsContainer => itemsContainer;
        public ItemsInventory ItemsInventory => itemsInventory;
        
        public override void Initialize(LevelSettings levelSettings)
        {
            base.Initialize(levelSettings);

            settings = levelSettings;
            
            inLevelDisposables = new CompositeDisposable();
            compositeDisposables = new CompositeDisposable();
            gameLevelResultData = new GameLevelResultData
            {
                IsWin = false,
                LevelResultState = ELevelResultState.None
            };
            
            levelCreator = new LevelCreator(itemsFactory);
            
            itemsContainer.AddTo(compositeDisposables);
            itemsInventory.AddTo(compositeDisposables);
            
            itemsInventory.EventInventoryFull.Subscribe(OnInventoryFull).AddTo(inLevelDisposables);
            itemsInventory.EventItemAddedToFreeSlot.Subscribe(OnEventItemAddedToFreeSlot).AddTo(inLevelDisposables);
            gameDataModel.EventLevelGoalCompleted.Subscribe(OnLevelGoalCompleted).AddTo(inLevelDisposables);
        }

        public override async void Show()
        {
            base.Show();
            SetLevelSpaceBounds();
            LevelPanel = await uiScreenManager.ShowUIPanelAsync<LevelPanel>();
            InitGameField();
            InitTimer();
            InitLevelPanel();
            InitTutor();

            CustomDebug.EventActionLevel.Subscribe(OnCustomDebugEventActionLevel).AddTo(inLevelDisposables);
        }

        public override void DeInitialize()
        {
            compositeDisposables.Dispose();
            inLevelDisposables.Dispose();
            uiScreenManager.HideUIPanel<LevelPanel>();
            
            base.DeInitialize();
        }

        private void InitLevelPanel()
        {
            LevelPanel.EventActivateBooster.Subscribe(OnActivateBooster).AddTo(inLevelDisposables);
            LevelPanel.EventMeinMenu.Subscribe(OnMainMenu).AddTo(inLevelDisposables);
            boostersController.EventBoosterActionStart.Subscribe(LevelPanel.EventBoosterActionStart).AddTo(inLevelDisposables);
            boostersController.EventBoosterActionFinished.Subscribe(LevelPanel.EventBoosterActionFinished).AddTo(inLevelDisposables);
            playerLoopProvider.EventLevelContinue.Subscribe(OnLevelContinue).AddTo(inLevelDisposables);
        }
        
        private void SetLevelSpaceBounds()
        {
            if (gameDataModel.IsCalculateGameFieldBounds)
            {
                return;
            }
            gameDataModel.IsCalculateGameFieldBounds = true;
            itemsContainer.SetLevelSpaceBounds(cameraManager.CameraConstantWidth, cameraManager.MainCamera);
            var backBoundPosition = itemsContainer.GetBackBoundPosition();
            itemsInventory.SetLocalPosition(new Vector3(0, 0, backBoundPosition.z - 0.5f));
        }

        private void InitGameField()
        {
            var levelBounds = itemsContainer.LevelBounds;
            var items = settings.Items;
            var itemsInstances = levelCreator.Create(levelBounds, items);
            
            itemsContainer.AddItemsRange(itemsInstances);
            itemsContainer.SetInteractableItems(false);
            itemsController = new ItemsController(itemsInstances, itemsInventory, itemsContainer, itemsParameters).AddTo(inLevelDisposables);
        }

        private void InitTimer()
        {
            timer.EventTimerFinish.Subscribe(OnTimerFinished).AddTo(inLevelDisposables);
            timer.Start(TimeSpan.FromSeconds(settings.Timer.TotalSeconds));
        }

        private async void InitTutor()
        {
            await gameTutorManager.TryShowTutorAsync(this);
            itemsContainer.SetInteractableItems(true);
        }

        private void OnActivateBooster(BoosterType boosterType)
        {
            boostersController.UseBooster(boosterType, false);
        }

        private void OnEventItemAddedToFreeSlot(string itemId)
        {
            gameDataModel.EventRemoveItemFromField.OnNext(itemId);
        }

        public override void Finish(LevelResultData levelResultData)
        {
            LevelPanel.DisableAllBoosters();
            LevelPanel.PauseWidget.SetInteractableButton(false);
            itemsContainer.SetInteractableItems(false);
            base.Finish(levelResultData);
        }

        private void OnInventoryFull(Unit unit)
        {
            gameLevelResultData.IsWin = false;
            gameLevelResultData.LevelResultState = ELevelResultState.InventoryFull;
            Finish(gameLevelResultData);
            timer.Pause();
        }

        private void OnTimerFinished(Unit unit)
        {
            gameLevelResultData.IsWin = false;
            gameLevelResultData.LevelResultState = ELevelResultState.TimeOver;
            Finish(gameLevelResultData);
            timer.Stop();
        }
        
        private void OnMainMenu(Unit unit)
        {
            gameLevelResultData.IsWin = false;
            gameLevelResultData.LevelResultState = ELevelResultState.ExitMenu;
            Finish(gameLevelResultData);
            timer.Stop();
        }
        
        private void OnLevelGoalCompleted(Unit unit)
        {
            gameLevelResultData.IsWin = true;
            gameLevelResultData.LevelResultState = ELevelResultState.None;
            Finish(gameLevelResultData);
            timer.Stop();
        }

        private void OnLevelContinue(ELevelResultState levelResultState)
        {
            switch (levelResultState)
            {
                case ELevelResultState.TimeOver:
                    timer.Start(TimeSpan.FromSeconds(resultPanelParameters.AddTimeForAds.TotalSeconds));
                    audioController.TryPlaySound(AudioNameData.EXTRA_TIME);
                    break;
                case ELevelResultState.InventoryFull:
                    timer.Continue();
                    itemsInventory.IsInventoryFull = false;
                    var itemCountInSlot = itemsInventory.GetItemsInSlots().Count;
                    for (int i = 0; i < itemCountInSlot; i++)
                    {
                        boostersController.UseBooster(BoosterType.Cancel, true);
                    }
                    break;
            }
            itemsContainer.SetInteractableItems(true);
            LevelPanel.BoostersWidget.SetInteractableButtons(true);
            LevelPanel.PauseWidget.SetInteractableButton(true);
        }

        private void OnCustomDebugEventActionLevel(EDebugLevelResultState levelResultState)
        {
            switch (levelResultState)
            {
                case EDebugLevelResultState.None:
                    OnLevelGoalCompleted(Unit.Default);
                    break;
                case EDebugLevelResultState.TimeOver:
                    OnTimerFinished(Unit.Default);
                    break;
                case EDebugLevelResultState.InventoryFull:
                    OnInventoryFull(Unit.Default);
                    break;
                case EDebugLevelResultState.ExitMenu:
                    OnMainMenu(Unit.Default);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(levelResultState), levelResultState, null);
            }
        }
    }
}