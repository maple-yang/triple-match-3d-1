using System;
using System.Collections.Generic;
using Game.Core.Components;
using Lean.Touch;
using UniRx;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.Core.Controllers
{
    public class LevelMapObserver : ILevelMapObserver, IDisposable
    {
        public ISubject<bool> EventLevelMapActive { get; set; }

        private bool isScrollActive;
        
        [Inject]
        private void Initialization()
        {
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerUp += OnFingerUp;
            EventLevelMapActive = new Subject<bool>();
        }
        
        public void Dispose()
        {
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerUp -= OnFingerUp;
        }
        
        private void OnFingerDown(LeanFinger leanFinger)
        {
            List<RaycastResult> raycastResults = LeanTouch.RaycastGui(leanFinger.ScreenPosition);

            if (raycastResults.Count <= 0)
            {
                return;
            }

            if (!raycastResults[0].gameObject.TryGetComponent(out LevelMapScroll s))
            {
                return;
            }

            if (!s.IsScrollActive)
            {
                return;
            }
            isScrollActive = true;
            EventLevelMapActive.OnNext(true);
        }

        private void OnFingerUp(LeanFinger leanFinger)
        {
            if (!isScrollActive)
            {
                return;
            }
            isScrollActive = false;
            EventLevelMapActive.OnNext(false);
        }
    }
}