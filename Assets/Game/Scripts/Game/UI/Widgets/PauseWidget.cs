using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Game.UI.Widgets
{
    public class PauseWidget : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        
        public ISubject<Unit> EventShowPausePanel;

        public void Show()
        {
            EventShowPausePanel = new Subject<Unit>();
            pauseButton.onClick.AddListener(OnShowPausePanel);
        }

        public void Hide()
        {
            pauseButton.onClick.RemoveListener(OnShowPausePanel);
        }
        
        public void SetInteractableButton(bool isInteractable)
        {
            pauseButton.interactable = isInteractable;
        }

        private void OnShowPausePanel()
        {
            EventShowPausePanel.OnNext(Unit.Default);
        }
    }
}