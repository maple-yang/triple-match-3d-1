using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core.Configurations;
using Game.Data.Models;
using Game.Systems.TimeSystem;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Widgets
{
    public class CompleteLevelWidget : MonoBehaviour
    {
        [SerializeField]
        private Button exitButton;
        
        [SerializeField]
        private List<Transform> openStarsTransforms;
        
        [SerializeField]
        private TextMeshProUGUI timerText;

        private Sequence sequence;
        
        public ISubject<Unit> EventExitButton;

        public async void Show(GameDataModel gameDataModel, ResultPanelParameters parameters, ITimer timer)
        {
            EventExitButton = new Subject<Unit>();
            exitButton.onClick.AddListener(OnExitButtonClick);
            gameObject.SetActive(true);
            
            timerText.text = timer.Time.Value.ToString(@"mm\:ss");
            openStarsTransforms.ForEach(t => t.gameObject.SetActive(false));

            await UniTask.Delay(500);

            sequence = DOTween.Sequence();
            var numberStars = gameDataModel.GetCurrentLevelProgressStars();
            for (int i = 0; i < numberStars; i++)
            {
                var starTransform = openStarsTransforms[i];
                starTransform.localScale = parameters.ShowStarsScale;
                sequence.Insert(0, starTransform.DOScale(Vector3.one, parameters.ShowStarsDuration / 2)
                    .SetEase(parameters.ShowStarsEase)
                    .SetDelay(i * parameters.ShowStarsDelayBetween)
                    .OnPlay(() =>
                    {
                        starTransform.gameObject.SetActive(true);
                    }));
                sequence.Insert(0, starTransform.DOLocalMove(Vector3.zero, parameters.ShowStarsDuration / 2)
                    .SetEase(parameters.ShowStarsEase)
                    .SetDelay(i * parameters.ShowStarsDelayBetween));
            }
        }

        public void Hide()
        {
            SetInteractableButtons(false);
            gameObject.SetActive(false);
            exitButton.onClick.RemoveListener(OnExitButtonClick);
            TryKillSequence();
        }

        private void OnExitButtonClick()
        {
            SetInteractableButtons(false);
            EventExitButton.OnNext(Unit.Default);
        }
        
        private void SetInteractableButtons(bool isInteractable)
        {
            exitButton.interactable = isInteractable;
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