using System;
using Lean.Touch;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Systems.PlayerInput
{
    public class GameInput : IGameInput, ITickable, IInitializable, IDisposable
    {
        private readonly ISubject<Vector2> onDown = new Subject<Vector2>();
        private readonly ISubject<Vector2> onUp = new Subject<Vector2>();
        private readonly ISubject<Vector2> onTap = new Subject<Vector2>();
        private readonly ISubject<Vector2> onDrag = new Subject<Vector2>();
        private readonly ISubject<Vector2> onStopDrag = new Subject<Vector2>();
        private readonly ISubject<(float, Vector2)> onZoom = new Subject<(float, Vector2)>();

        private bool isPressed;
        private IDisposable clickTimerSubscriber;

        public bool IsEnabled { get; private set; }
        public bool IsTapActive { get; private set; }

        public IObservable<Vector2> OnDown => onDown;
        public IObservable<Vector2> OnUp => onUp;
        public IObservable<Vector2> OnTap => onTap;
        public IObservable<Vector2> OnDrag => onDrag;
        public IObservable<Vector2> OnStopDrag => onStopDrag;
        public IObservable<(float, Vector2)> OnZoom => onZoom;

        public void Tick()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (Input.touchCount == 2)
            {
                var touchZero = Input.GetTouch(0);
                var touchOne = Input.GetTouch(1);
                var center = (touchOne.position + touchZero.position) / 2;

                var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                var prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                var currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                var difference = currentMagnitude - prevMagnitude;

                HandleZoom(difference * 0.05f, center);
            }
            else
            {
                HandleZoom(Input.GetAxis("Mouse ScrollWheel"), Input.mousePosition);
            }
        }

        public void Initialize()
        {
            LeanTouch.OnFingerDown += HandleDown;
            LeanTouch.OnFingerUp += HandleUp;
            LeanTouch.OnFingerTap += HandleTap;
            LeanTouch.OnFingerUpdate += HandleDrag;
        }
        
        
        public void Dispose()
        {
            LeanTouch.OnFingerDown -= HandleDown;
            LeanTouch.OnFingerUp -= HandleUp;
            LeanTouch.OnFingerTap -= HandleTap;
            LeanTouch.OnFingerUpdate -= HandleDrag;
            
            clickTimerSubscriber?.Dispose();
            clickTimerSubscriber = null;

            IsTapActive = false;
        }

        public void SetEnabled(bool isEnabled)
        {
            if (IsEnabled == isEnabled)
            {
                return;
            }

            if (!isEnabled)
            {
                isPressed = false;
            }

            IsEnabled = isEnabled;
            IsTapActive = false;
        }

        // Events

        private void HandleDown(LeanFinger data)
        {
            if (!IsEnabled || data.IsOverGui)
            {
                return;
            }

            onDown.OnNext(data.ScreenPosition);
            isPressed = true;

            IsTapActive = true;
            clickTimerSubscriber = Observable.Timer(TimeSpan.FromSeconds(LeanTouch.Instance.TapThreshold)).Subscribe(_ => IsTapActive = false);
        }

        private void HandleUp(LeanFinger data)
        {
            isPressed = false;

            if (!IsEnabled)
            {
                return;
            }

            onUp.OnNext(data.ScreenPosition);
            
            clickTimerSubscriber?.Dispose();
            clickTimerSubscriber = null;
        }

        private void HandleTap(LeanFinger data)
        {
            if (!IsEnabled || data.IsOverGui || !isPressed || !IsTapActive)
            {
                return;
            }

            onTap.OnNext(data.ScreenPosition);
        }

        private void HandleDrag(LeanFinger data)
        {
            if (!IsEnabled || !isPressed)
            {
                return;
            }

            if (Input.touchCount > 1)
            {
                onStopDrag.OnNext(data.ScreenPosition);
                return;
            }

            var screenDelta = data.ScreenDelta.magnitude;
            if (screenDelta > 0.1f)
            {
                IsTapActive = false;
                onDrag.OnNext(data.ScreenPosition);
            }
        }

        private void HandleZoom(float difference, Vector2 center)
        {
            if (difference == 0)
            {
                return;
            }

            isPressed = false;
            onZoom.OnNext((difference, center));
        }
    }
}