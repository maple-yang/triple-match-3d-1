using System;
using DG.Tweening;
using Game.Systems.TimeSystem;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Game.UI.Widgets
{
    public class TimerWidget : MonoBehaviour
    {
        [SerializeField]
        private Text timerText;
        
        [SerializeField]
        private Color startBlinkColor = Color.white;
        
        [SerializeField]
        private Color endBlinkColor = Color.red;
        
        [SerializeField]
        private float blinkDuration = 1f;

        private ITimer timer;
        private CompositeDisposable compositeDisposables;
        private bool isBlinkingTextAnimation;

        public void Show(ITimer iTimer)
        {
            compositeDisposables = new CompositeDisposable();
            isBlinkingTextAnimation = false;

            timer = iTimer;
            timer.Time.Subscribe(OnTickTimer).AddTo(compositeDisposables);
        }

        public void Hide()
        {
            timer.Stop();
            compositeDisposables.Clear();
            DOTween.Kill(this);
        }

        private void OnTickTimer(TimeSpan timeSpan)
        {
            timerText.text = timeSpan.ToString(@"mm\:ss");
            if (timeSpan.TotalSeconds <= 30 && !isBlinkingTextAnimation && timeSpan.TotalSeconds != 0)
            {
                isBlinkingTextAnimation = true;
                BlinkingTextAnimation();
            }
            else
            {
                if (timeSpan.TotalSeconds > 30 && isBlinkingTextAnimation)
                {
                    isBlinkingTextAnimation = false;
                    DOTween.Kill(this);
                    timerText.color = startBlinkColor;
                }
            }
        }

        private void BlinkingTextAnimation()
        {
            Sequence sequence = DOTween.Sequence().SetId(this).SetLoops(-1, LoopType.Restart);
            sequence
                .AppendCallback(() => timerText.color = startBlinkColor)
                .Append(timerText.DOColor(endBlinkColor, blinkDuration).SetEase(Ease.Linear))
                .Append(timerText.DOColor(startBlinkColor, blinkDuration).SetEase(Ease.Linear));
        }
        
        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}