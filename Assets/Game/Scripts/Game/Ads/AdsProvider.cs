using System;
using YG;
using Zenject;

namespace Game.Ads
{
    public class AdsProvider : IAdsProvider, IDisposable
    {
        public AdsProvider(
            AdsMessageController adsMessageController)
        {
            this.adsMessageController = adsMessageController;
        }

        private readonly AdsMessageController adsMessageController;
        private event Action<bool> EventVideoEventResult;
        private event Action EventInterstitialClose;

        [Inject]
        private void Initialization()
        {
            YandexGame.RewardVideoEvent += OnRewardVideoEvent;
            YandexGame.CloseVideoEvent += OnCloseVideoEvent;
            YandexGame.ErrorVideoEvent += OnErrorVideoEvent;
            YandexGame.CloseFullAdEvent += OnCloseFullAdEvent;
            YandexGame.ErrorFullAdEvent += OnErrorFullAdEvent;
        }
        
        public void ShowRewardVideo(Action<bool> eventRewardVideoShown)
        {
            EventVideoEventResult = eventRewardVideoShown;
            YandexGame.RewVideoShow(0);
        }

        public bool ShowInterstitial(Action eventInterstitialClose = null)
        {
            EventInterstitialClose = eventInterstitialClose;
            return !YandexGame.FullscreenShow();
        }

        public void Dispose()
        {
            YandexGame.RewardVideoEvent -= OnRewardVideoEvent;
            YandexGame.CloseVideoEvent -= OnCloseVideoEvent;
            YandexGame.ErrorVideoEvent -= OnErrorVideoEvent;
            YandexGame.CloseFullAdEvent -= OnCloseFullAdEvent;
            YandexGame.ErrorFullAdEvent -= OnErrorFullAdEvent;
        }

        private void OnRewardVideoEvent(int id)
        {
            EventVideoEventResult?.Invoke(true);
        }
        
        private void OnCloseVideoEvent()
        {
        }
        
        private void OnErrorVideoEvent()
        {
            EventVideoEventResult?.Invoke(false);
            adsMessageController.ShowErrorMessage();
        }
        
        private void OnCloseFullAdEvent()
        {
            EventInterstitialClose?.Invoke();
        }
        
        private void OnErrorFullAdEvent()
        {
            EventInterstitialClose?.Invoke();
        }
    }
}