using TMPro;
using UnityEngine;

namespace Game.Modules.TutorModule.View
{
    public class TutorMessage : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI messageText;
        
        public void Show(Vector3 position, Vector2 offset)
        {
            gameObject.SetActive(true);
            
            transform.position = position;
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.anchoredPosition += offset;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ChangeMessageSize(Vector2 sizeDelta)
        {
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.sizeDelta = sizeDelta;
        }
    }
}