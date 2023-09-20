using Game.Data.Models;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI.Components
{
    public class BoosterButton : MonoBehaviour
    {
        [SerializeField]
        private BoosterType boosterType;
        
        [SerializeField]
        private Button boosterButton;
        
        [SerializeField]
        private Button extraButton;

        [SerializeField]
        private TextMeshProUGUI counter;

        [SerializeField]
        private GameObject counterRoot;
        
        [SerializeField]
        private GameObject extraRoot;
        
        [SerializeField]
        private GameObject openState;
        
        [SerializeField]
        private GameObject lockState;
        
        [SerializeField]
        private TextMeshProUGUI openOnLevelText;

        private CompositeDisposable compositeDisposables;

        public readonly ISubject<BoosterButton> EventBoosterButton = new Subject<BoosterButton>();
        public readonly ISubject<BoosterButton> EventExtraButton = new Subject<BoosterButton>();

        public BoosterType BoosterType => boosterType;

        public void Initialize(IntReactiveProperty counterReactiveProperty)
        {
            compositeDisposables = new CompositeDisposable();
            counterReactiveProperty.Subscribe(OnCounterChanged).AddTo(compositeDisposables);
            
            boosterButton.onClick.AddListener(OnBoosterButtonClick);
            extraButton.onClick.AddListener(OnExtraButtonClick);
            
            SetActiveState(true);
        }

        public void DeInitialize()
        {
            compositeDisposables?.Dispose();
            
            boosterButton.onClick.RemoveListener(OnBoosterButtonClick);
            extraButton.onClick.RemoveListener(OnExtraButtonClick);
        }

        public void Lock(int openOnLevel)
        {
            SetActiveState(false);
            openOnLevelText.text = $"{openOnLevel}";
        }

        public void SetInteractableButtons(bool isInteractable)
        {
            boosterButton.interactable = isInteractable;
            extraButton.interactable = isInteractable;
        }

        private void OnCounterChanged(int count)
        {
            SetCounter(count);
            SetActiveCounter(count > 0);
        }
        
        private void SetCounter(int count)
        {
            counter.text = count.ToString();
        }

        private void SetActiveCounter(bool isActive)
        {
            counterRoot.SetActive(isActive);
            extraRoot.SetActive(!isActive);
        }
        
        private void SetActiveState(bool isOpen)
        {
            openState.SetActive(isOpen);
            lockState.SetActive(!isOpen);
        }
        
        private void OnBoosterButtonClick()
        {
            EventBoosterButton.OnNext(this);
        }
        
        private void OnExtraButtonClick()
        {
            EventExtraButton.OnNext(this);
        }
    }
}