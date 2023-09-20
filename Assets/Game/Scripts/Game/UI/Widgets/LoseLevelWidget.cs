using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Core.Configurations;
using Game.Data;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Widgets
{
    public class LoseLevelWidget : MonoBehaviour
    {
        [SerializeField]
        private Button exitButton;
        
        [SerializeField]
        private Button continueButton;

        [SerializeField]
        private GameObject timeOverContent;
        
        [SerializeField]
        private TextMeshProUGUI timeOverContentText;
        
        [SerializeField]
        private GameObject inventoryFullContent;

        private ELevelResultState typeLevelResultState;
        
        public ISubject<Unit> EventExitButton;
        public ISubject<ELevelResultState> EventContinueButton;

        public void Show(ELevelResultState levelResultState, ResultPanelParameters resultPanelParameters)
        {
            typeLevelResultState = levelResultState;
            EventExitButton = new Subject<Unit>();
            EventContinueButton = new Subject<ELevelResultState>();
            
            if (typeLevelResultState == ELevelResultState.ExitMenu)
            {
                Hide();
                return;
            }

            timeOverContent.SetActive(typeLevelResultState == ELevelResultState.TimeOver);
            inventoryFullContent.SetActive(typeLevelResultState == ELevelResultState.InventoryFull);
            var addTimeForAdsTimeSpan = TimeSpan.FromSeconds(resultPanelParameters.AddTimeForAds.TotalSeconds);
            var addTimeForAdsText = addTimeForAdsTimeSpan.ToString(@"mm\:ss");
            timeOverContentText.text = $"+{addTimeForAdsText}";
            
            exitButton.onClick.AddListener(OnExitButtonClick);
            continueButton.onClick.AddListener(OnContinueButtonClick);
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            SetInteractableButtons(false);
            gameObject.SetActive(false);
            exitButton.onClick.RemoveListener(OnExitButtonClick);
            continueButton.onClick.RemoveListener(OnContinueButtonClick);
        }

        private void OnExitButtonClick()
        {
            SetInteractableButtons(false);
            EventExitButton.OnNext(Unit.Default);
        }

        private void OnContinueButtonClick()
        {
            SetInteractableButtons(false);
            EventContinueButton.OnNext(typeLevelResultState);
        }
        
        public void SetInteractableButtons(bool isInteractable)
        {
            exitButton.interactable = isInteractable;
            continueButton.interactable = isInteractable;
        }
    }
}