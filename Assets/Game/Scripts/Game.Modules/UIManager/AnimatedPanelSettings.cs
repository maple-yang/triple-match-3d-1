using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Modules.UIManager
{
    [Serializable]
    public class AnimatedPanelSettings
    {
        [field: SerializeField]
        public CanvasGroup CanvasGroup { get; private set; }

        // Show

        [field: SerializeField, Min(0)]
        public float ShowDelay { get; private set; } = 0;

        [field: SerializeField, Min(0)]
        public float ShowDuration { get; private set; } = 0.5f;

        [field: SerializeField]
        public bool UseCustomShowEase { get; private set; } = false;

        [field: SerializeField, HideIf(nameof(UseCustomShowEase))]
        public Ease ShowEase { get; private set; } = Ease.Linear;

        [field: SerializeField, ShowIf(nameof(UseCustomShowEase))]
        public AnimationCurve CustomShowEase { get; private set; } = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // Hide

        [field: SerializeField, Min(0)]
        public float HideDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        public bool UseCustomHideEase { get; private set; } = false;
        
        [field: SerializeField, HideIf(nameof(UseCustomHideEase))]
        public Ease HideEase { get; private set; } = Ease.Linear;
        
        [field: SerializeField, ShowIf(nameof(UseCustomHideEase))]
        public AnimationCurve CustomHideEase { get; private set; } = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }
}