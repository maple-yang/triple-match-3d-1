using TMPro;
using UnityEngine;

namespace Game.UI.Widgets
{
    public class LevelNumberWidget : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI levelNumberText;
    
        [SerializeField] 
        private string format = "Level {0}";
    
        public void Show(int levelId)
        {
            levelNumberText.text = string.Format(format, levelId);
        }
    }
}
