using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Modules.UIManager
{
    public class AnimatedPanel : AnimatedPanel<UIContext>
    {
    }

    [Serializable]
    public class AnimatedPanel<TPanelContext> : UIPanel<TPanelContext> where TPanelContext : UIContext
    {
        [SerializeField]
        private AnimatedPanelSettings animatedPanelSettings;

        [SerializeField]
        private string layerID = "default_layer";
        
        private bool isActive = false;

        protected TPanelContext PanelContext { get; private set; }

        public override bool IsActive => isActive;
        public override string LayerID => layerID;

        public override void Initialize(TPanelContext panelContext = default(TPanelContext))
        {
            PanelContext = panelContext;
            isActive = false;
        }

        public override void DeInitialize()
        {
            DOTween.Kill(this);
        }

        public override void Show(bool instant = false,Action onShowed = null)
        {
            DOTween.Kill(this);
            var canvasGroup = animatedPanelSettings.CanvasGroup;
            if (instant)
            {
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
                isActive = true;
                
                onShowed?.Invoke();
            }
            else
            {
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;

                var showDelay = animatedPanelSettings.ShowDelay;
                var showDuration = animatedPanelSettings.ShowDuration;
                var ease = animatedPanelSettings.ShowEase;
                var customEase = animatedPanelSettings.CustomShowEase;
                var tween = canvasGroup.DOFade(1, showDuration).SetDelay(showDelay).SetId(this);
                
                if (animatedPanelSettings.UseCustomShowEase)
                {
                    tween.SetEase(customEase);
                }
                else
                {
                    tween.SetEase(ease);
                }

                tween.Play();
                tween.OnComplete(() =>
                {
                    canvasGroup.blocksRaycasts = true;
                    isActive = true;
                    
                    onShowed?.Invoke();
                });
            }
        }

        public override void UpdatePanel(TPanelContext newPanelContext = default(TPanelContext))
        {
            PanelContext = newPanelContext;
        }

        public override void Hide(bool instant = false, Action onHidden = null)
        {
            DOTween.Kill(this);
            var canvasGroup = animatedPanelSettings.CanvasGroup;
            canvasGroup.blocksRaycasts = false;

            if (instant)
            {
                canvasGroup.alpha = 0;
                isActive = false;
                
                onHidden?.Invoke();
            }
            else
            {
                var hideDuration = animatedPanelSettings.HideDuration;
                var ease = animatedPanelSettings.HideEase;
                var customEase = animatedPanelSettings.CustomHideEase;
                var tween = canvasGroup.DOFade(0, hideDuration).SetId(this);
                
                if (animatedPanelSettings.UseCustomHideEase)
                {
                    tween.SetEase(customEase);
                }
                else
                {
                    tween.SetEase(ease);
                }

                tween.Play();
                tween.OnComplete(() =>
                {
                    isActive = false;
                    
                    onHidden?.Invoke();
                });
            }
        }
    }
}
