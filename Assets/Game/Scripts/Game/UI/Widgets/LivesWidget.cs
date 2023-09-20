using System;
using Game.Ads;
using Game.Modules.LivesManager;
using Game.Providers;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI.Widgets
{
    public class LivesWidget : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI livesCountText;
        
        [SerializeField] 
        private TextMeshProUGUI timerRechargeLivesText;
        
        [SerializeField] 
        private Transform timerRechargeLivesTransform;
        
        [SerializeField] 
        private Transform fullLivesTransform;
        
        [SerializeField] 
        private Transform addLivesTransform;
        
        [SerializeField] 
        private Button addLivesButton;
        
        [SerializeField] 
        private Transform addLivesPopupTransform;
        
        [SerializeField] 
        private Button addLivesPopupButton;
        
        [SerializeField] 
        private Button closePopupButton;
        
        [Inject] 
        private ILivesProvider livesProvider;
        
        [Inject] 
        private IAdsProvider adsProvider;

        private CompositeDisposable compositeDisposable;

        public void Show()
        {
            compositeDisposable = new CompositeDisposable();
            livesProvider.EventLivesDataUpdated.Subscribe(OnLivesDataUpdated).AddTo(compositeDisposable);
            livesProvider.EventLivesCountFull.Subscribe(OnLivesCountFull).AddTo(compositeDisposable);
            livesProvider.TimeRechargeLives.Subscribe(OnTimeRechargeLives).AddTo(compositeDisposable);
            livesProvider.UpdateLivesView();
            addLivesButton.onClick.AddListener(OnClickAddLivesButton);
            OnHideAddLivesPopup();
        }

        public void Hide()
        {
            compositeDisposable.Dispose();
            compositeDisposable.Clear();
            addLivesButton.onClick.RemoveListener(OnClickAddLivesButton);
        }

        private void OnLivesDataUpdated(LivesData livesData)
        {
            livesCountText.text = $"{livesData.LivesCount}";
        }
        
        private void OnLivesCountFull(bool isLivesCountFull)
        {
            fullLivesTransform.gameObject.SetActive(isLivesCountFull);
            timerRechargeLivesTransform.gameObject.SetActive(!isLivesCountFull);
            addLivesTransform.gameObject.SetActive(!isLivesCountFull);
        }
        
        private void OnTimeRechargeLives(TimeSpan timeSpan)
        {
            timerRechargeLivesText.text = timeSpan.ToString(@"mm\:ss");
        }

        private void OnClickAddLivesButton()
        {
            SetInteractableButton(false);
            adsProvider.ShowRewardVideo((succeed) =>
            {
                if (succeed)
                {
                    livesProvider.AddLives();
                    OnHideAddLivesPopup();
                }
                SetInteractableButton(true);
            });
        }

        public void ShowAddLivesPopup()
        {
            addLivesPopupButton.onClick.AddListener(OnClickAddLivesButton);
            closePopupButton.onClick.AddListener(OnHideAddLivesPopup);
            addLivesPopupTransform.gameObject.SetActive(true);
        }
        
        private void OnHideAddLivesPopup()
        {
            addLivesPopupTransform.gameObject.SetActive(false);
            addLivesPopupButton.onClick.RemoveListener(OnClickAddLivesButton);
            closePopupButton.onClick.RemoveListener(OnHideAddLivesPopup);
        }

        private void SetInteractableButton(bool isInteractable)
        {
            addLivesButton.interactable = isInteractable;
            addLivesPopupButton.interactable = isInteractable;
            closePopupButton.interactable = isInteractable;
        }
    }
}