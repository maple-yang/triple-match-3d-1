using System;
using Lean.Touch;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Modules.CurveMapModule
{
    public class CurveMapObserver : ICurveMapObserver, IDisposable
    {
        public ISubject<Vector2> EventCurveMapSwipe { get; set; }

        private bool isActive = true;
        
        [Inject]
        private void Initialization()
        {
            LeanTouch.OnFingerSwipe += OnFingerSwipe;
            EventCurveMapSwipe = new Subject<Vector2>();
        }
        
        public void Dispose()
        {
            LeanTouch.OnFingerSwipe -= OnFingerSwipe;
        }

        private void OnFingerSwipe(LeanFinger leanFinger)
        {
            if (!isActive)
            {
                return;   
            }
            var swipeDirection = leanFinger.SwipeScreenDelta.normalized;
            EventCurveMapSwipe.OnNext(swipeDirection);
        }

        public void SetActiveSwipe(bool isActiveSwipe)
        {
            isActive = isActiveSwipe;
        }
    }
}