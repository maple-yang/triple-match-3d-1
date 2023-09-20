using System;
using System.Linq;
using Game.Data.Models;
using Game.UI.Components;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;

namespace Game.UI.Widgets
{
    public class BoostersWidget : MonoBehaviour
    {
        [SerializeField]
        private BoosterButton[] boosters;
        
        [SerializeField]
        private BoosterEffect[] boosterEffects;

        private CompositeDisposable compositeDisposables;

        public BoosterButton[] BoosterView => boosters;
        public ISubject<BoosterType> EventBoosterActivate;
        public ISubject<BoosterType> EventExtraBoosterActivate;

        public void Show(BoostersData[] boostersData, int lastOpenedLevelIndex)
        {
            EventBoosterActivate = new Subject<BoosterType>();
            EventExtraBoosterActivate = new Subject<BoosterType>();
            compositeDisposables = new CompositeDisposable();
            
            foreach (var booster in boostersData)
            {
                var boosterView = boosters.SingleOrDefault(t => t.BoosterType == booster.BoosterType);
                if (boosterView == null)
                {
                    Debug.LogError($"There's no view for booster \'{Enum.GetName(typeof(BoosterType), booster.BoosterType)}\'");
                    continue;
                }
                
                if (booster.OpenOnLevel.Value > lastOpenedLevelIndex + 1)
                {
                    boosterView.Lock(booster.OpenOnLevel.Value);
                    continue;
                }

                boosterView.Initialize(booster.Counter);
                boosterView.EventBoosterButton.Subscribe(OnBoosterClick).AddTo(compositeDisposables);
                boosterView.EventExtraButton.Subscribe(OnBoosterExtraClick).AddTo(compositeDisposables);
            }
        }

        public void Hide()
        {
            boosters.ForEach(t => t.DeInitialize());
            
            compositeDisposables.Dispose();
            compositeDisposables = null;
        }

        public void ShowBoosterEffect(BoosterType boosterType)
        {
            var boosterEffect = boosterEffects.FirstOrDefault(t => t.BoosterType == boosterType);
            if(boosterEffect == null)
            {
                return;
            }
            boosterEffect.ShowEffect();
        }
        
        public void HideBoosterEffect(BoosterType boosterType)
        {
            var boosterEffect = boosterEffects.FirstOrDefault(t => t.BoosterType == boosterType);
            if(boosterEffect == null)
            {
                return;
            }
            boosterEffect.HideEffect();
        }

        public void SetInteractableButtons(bool isInteractable)
        {
            boosters.ForEach(t => t.SetInteractableButtons(isInteractable));
        }

        private void OnBoosterClick(BoosterButton boosterButton)
        {
            EventBoosterActivate.OnNext(boosterButton.BoosterType);
        }

        private void OnBoosterExtraClick(BoosterButton boosterButton)
        {
            EventExtraBoosterActivate.OnNext(boosterButton.BoosterType);
        }
    }
}
