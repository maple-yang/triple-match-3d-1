using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components
{
    public class ProgressBar : MonoBehaviour
    {
        public enum FillType
        {
            Horizontal = 0,
            Vertical = 1,
            FillAmount = 2
        }
        
        [SerializeField, ShowIf("fillType", FillType.FillAmount)]
        private Image imageFillAmount;
        
        [SerializeField, HideIf("fillType", FillType.FillAmount)]
        private RectTransform rectFill;
        
        [SerializeField]
        private Transform roundRectFill;
        
        [SerializeField]
        private FillType fillType = FillType.Horizontal;

        [SerializeField]
        private float fillDuration = 0.25f;
        
        [SerializeField]
        private Ease fillEase = Ease.OutCirc;

        private Sequence sequence;

        private void OnDestroy()
        {
            DOTween.Kill(this);
            sequence.Kill();
        }

        public virtual void SetNormalizedProgress(float progress, bool instant = false)
        {
            var currentAnchor = rectFill.anchorMax;

            switch (fillType)
            {
                case FillType.Horizontal:
                    currentAnchor.x = progress;
                    break;
                case FillType.Vertical:
                    currentAnchor.y = progress;
                    break;
                case FillType.FillAmount:
                    FillAmountProgress(progress, instant);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            FillProgressAnchor(currentAnchor, instant);
        }

        protected virtual void FillProgressAnchor(Vector2 targetAnchor, bool instant = false)
        {
            DOTween.Kill(this);

            if (instant)
            {
                rectFill.anchorMax = targetAnchor;
            }
            else
            {
                rectFill.DOAnchorMax(targetAnchor, fillDuration).SetEase(fillEase).SetId(this);
            }
        }
        
        protected virtual void FillAmountProgress(float targetValue, bool instant = false)
        {
            DOTween.Kill(this);

            if (instant)
            {
                imageFillAmount.fillAmount = targetValue;
            }
            else
            {
                imageFillAmount.DOFillAmount(targetValue, fillDuration).SetEase(fillEase).SetId(this);
            }
        }

        public void PlayLoadingAnimation()
        {
            sequence = DOTween.Sequence();
            sequence.Append(roundRectFill.DOLocalRotate(new Vector3(0,0, -360), 1f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart);
        }
    }
}