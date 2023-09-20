using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Game.UI.Widgets
{
    public class CustomToggleWidget : MonoBehaviour
    {
        [SerializeField] private Transform offContainer;
        [SerializeField] private Transform onContainer;
        [SerializeField] private Button toggleButton;
        
        private bool isOn;

        public ISubject<BoolReactiveProperty> EventToggleChanged;

        public void Initialized(BoolReactiveProperty isOn)
        {
            EventToggleChanged = new Subject<BoolReactiveProperty>();
            toggleButton.onClick.AddListener(OnEventToggleChanged);
            this.isOn = isOn.Value;
            SetToggle(isOn);
        }
        
        public void DeInitialized()
        {
            toggleButton.onClick.RemoveListener(OnEventToggleChanged);
        }

        private void SetToggle(BoolReactiveProperty isOn)
        {
            offContainer.gameObject.SetActive(!isOn.Value);
            onContainer.gameObject.SetActive(isOn.Value);
        }

        private void OnEventToggleChanged()
        {
            isOn = !isOn;
            var isToggleOn = new BoolReactiveProperty(isOn);
            SetToggle(isToggleOn);
            EventToggleChanged.OnNext(isToggleOn);
        }
    }
}
