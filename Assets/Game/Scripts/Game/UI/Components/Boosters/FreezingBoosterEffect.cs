using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Data.Models;
using Game.Modules.AudioManager;
using UnityEngine;
using Zenject;

namespace Game.UI.Components
{
    public class FreezingBoosterEffect : BoosterEffect
    {
        [SerializeField] 
        private CanvasGroup canvasGroup;
        
        [SerializeField] 
        private List<ParticleSystem> FreezeTimeItems = new List<ParticleSystem>();

        private Sequence sequence;

        public override void ShowEffect()
        {
            audioController.TryPlaySound(AudioNameData.BOOSTER_FREEZE_START);
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 0;
            sequence = DOTween.Sequence();
            sequence.Append(canvasGroup.DOFade(1, 0.5f));
            FreezeTimeItems.ForEach(f => f.Play());
        }

        public override void HideEffect()
        {
            audioController.TryPlaySound(AudioNameData.BOOSTER_FREEZE_END);
            FreezeTimeItems.ForEach(f => f.Stop());
            canvasGroup.alpha = 1;
            sequence = DOTween.Sequence();
            sequence.Append(canvasGroup.DOFade(0, 1f));
            sequence.OnKill(() =>
                {
                    canvasGroup.gameObject.SetActive(false);
                });
        }
        
        private void TryKillSequence()
        {
            if (sequence != null)
            {
                sequence.Kill();
                sequence = null;
            }
        }

        private void OnDestroy()
        {
            TryKillSequence();
        }
    }
}