using System;
using Game.Data.Models;
using YG;

namespace Game.Providers.Language
{
    public class LanguageProvider : ILanguageProvider, IDisposable
    {
        public LanguageProvider(
            GameDataModel gameDataModel)
        {
            this.gameDataModel = gameDataModel;
        }

        private readonly GameDataModel gameDataModel;
        private string currentLanguage;

        public void Initialization()
        {
            currentLanguage = string.IsNullOrEmpty(gameDataModel.Language)
                ? YandexGame.savesData.language
                : gameDataModel.Language;
            if (string.IsNullOrEmpty(gameDataModel.Language))
            {
                gameDataModel.Language = currentLanguage;
                gameDataModel.Save();
            }
        }

        public string GetLanguage()
        {
            return currentLanguage;
        }

        public void SetLanguage(string language)
        {
            currentLanguage = language;
            YandexGame.SwitchLanguage(currentLanguage);
            gameDataModel.Language = currentLanguage;
            gameDataModel.Save();
        }

        public void Dispose()
        {
        }
    }
}