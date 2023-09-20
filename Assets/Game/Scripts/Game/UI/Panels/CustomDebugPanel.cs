using System;
using Game.Levels;
using Game.Modules.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Panels
{
    public class CustomDebugPanel : AnimatedPanel
    {
        [SerializeField]
        private Button openLogButton;
        
        [SerializeField]
        private Button levelCompletedButton;
        
        [SerializeField]
        private Button levelTimeOverButton;
        
        [SerializeField]
        private Button levelInventoryFullButton;
        
        [SerializeField]
        private Button levelExitMenuButton;
        
        [SerializeField]
        private GameObject logContainer;
        
        [SerializeField]
        private TextMeshProUGUI logText;
        
        [SerializeField]
        private ScrollRect scrollRect;
        
        private bool isLogContainerOpen;

        public override void Show(bool instant = false, Action onShowed = null)
        {
            base.Show(instant, onShowed);
            openLogButton.onClick.AddListener(OnShowDebugLog);
            levelCompletedButton.onClick.AddListener(OnLevelCompleted);
            levelTimeOverButton.onClick.AddListener(OnLevelTimeOver);
            levelInventoryFullButton.onClick.AddListener(OnLevelInventoryFull);
            levelExitMenuButton.onClick.AddListener(OnLevelExitMenu);
        }

        public override void Hide(bool instant = false, Action onHidden = null)
        {
            openLogButton.onClick.RemoveListener(OnShowDebugLog);
            levelCompletedButton.onClick.RemoveListener(OnLevelCompleted);
            levelTimeOverButton.onClick.RemoveListener(OnLevelTimeOver);
            levelInventoryFullButton.onClick.RemoveListener(OnLevelInventoryFull);
            levelExitMenuButton.onClick.RemoveListener(OnLevelExitMenu);
            base.Hide(instant, onHidden);
        }

        private void OnShowDebugLog()
        {
            isLogContainerOpen = !isLogContainerOpen;
            logContainer.SetActive(isLogContainerOpen);
            if (isLogContainerOpen)
            {
                logText.text = CustomDebug.GetLog();
                scrollRect.verticalNormalizedPosition = 0;
            }
        }

        private void OnLevelCompleted()
        {
            CustomDebug.ActionLevel(EDebugLevelResultState.None);
            OnShowDebugLog();
        }
        
        private void OnLevelTimeOver()
        {
            CustomDebug.ActionLevel(EDebugLevelResultState.TimeOver);
            OnShowDebugLog();
        }
        
        private void OnLevelInventoryFull()
        {
            CustomDebug.ActionLevel(EDebugLevelResultState.InventoryFull);
            OnShowDebugLog();
        }
        
        private void OnLevelExitMenu()
        {
            CustomDebug.ActionLevel(EDebugLevelResultState.ExitMenu);
            OnShowDebugLog();
        }
    }
}