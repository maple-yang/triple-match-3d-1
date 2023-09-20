using System.Collections.Generic;
using Game.Providers.Language;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Widgets
{
    public class LanguageWidget : MonoBehaviour
    {
        [SerializeField] 
        private Button languageButton;

        [SerializeField] 
        private TextMeshProUGUI languageText;
        
        [SerializeField] 
        private Image languageImage;
        
        [SerializeField] 
        private List<Sprite> languageSprites;

        private string[] languages = new string[]
        {
            "ru",
            "en",
            "tr"
        };
        
        private string[] nameLanguages = new string[]
        {
            "Русский",
            "English",
            "Türkçe"
        };

        private int currentLanguageId = 0;
        private ILanguageProvider languageProvider;

        public void Initialized(ILanguageProvider languageProvider)
        {
            this.languageProvider = languageProvider;
            languageButton.onClick.AddListener(OnSwitchLanguage);

            currentLanguageId = 0;
            var currentLanguage = this.languageProvider.GetLanguage();
            for (int i = 0; i < languages.Length; i++)
            {
                var language = languages[i];
                if (language == currentLanguage)
                {
                    currentLanguageId = i;
                    break;
                }
            }
            SwitchLanguage();
        }
        
        public void DeInitialized()
        {
            languageButton.onClick.RemoveListener(OnSwitchLanguage);
        }

        private void OnSwitchLanguage()
        {
            currentLanguageId++;
            if (currentLanguageId >= languages.Length)
            {
                currentLanguageId = 0;
            }

            SwitchLanguage();
        }
        
        private void SwitchLanguage()
        {
            languageText.text = nameLanguages[currentLanguageId];
            languageProvider.SetLanguage(languages[currentLanguageId]);
        }
    }
}
