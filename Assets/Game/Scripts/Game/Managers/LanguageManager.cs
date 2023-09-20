using System;
using Game.Providers;
using Game.Providers.Language;
using UniRx;
using Zenject;

namespace Game.Managers
{
    public class LanguageManager : IDisposable
    {
        private readonly ILanguageProvider languageProvider;
        private readonly IPlayerLoopProvider playerLoopProvider;
        private CompositeDisposable compositeDisposable; 
        
        public LanguageManager(
            ILanguageProvider languageProvider,
            IPlayerLoopProvider playerLoopProvider)
        {
            this.languageProvider = languageProvider;
            this.playerLoopProvider = playerLoopProvider;
        }
        
        [Inject]
        private void Initialization()
        {
            compositeDisposable = new CompositeDisposable();
            playerLoopProvider.EventGameLoaded.Subscribe(OnGameLoaded).AddTo(compositeDisposable);
        }

        private void OnGameLoaded(Unit unit)
        {
            languageProvider.Initialization();
        }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }
    }
}