namespace Game.Providers.Language
{
    public interface ILanguageProvider
    {
        void Initialization();
        string GetLanguage();
        void SetLanguage(string language);
    }
}