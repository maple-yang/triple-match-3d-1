using System;

namespace Game.Ads
{
    public interface IAdsProvider
    {
        public void ShowRewardVideo(Action<bool> eventRewardVideoShown);
        public bool ShowInterstitial(Action eventInterstitialClose = null);
    }
}