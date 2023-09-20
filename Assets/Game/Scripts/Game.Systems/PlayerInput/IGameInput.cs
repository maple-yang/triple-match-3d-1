using System;
using UnityEngine;

namespace Game.Systems.PlayerInput
{
    public interface IGameInput
    {
        IObservable<Vector2> OnDown { get; }
        IObservable<Vector2> OnUp { get; }
        IObservable<Vector2> OnTap { get; }
        IObservable<Vector2> OnDrag { get; }
        IObservable<Vector2> OnStopDrag { get; }
        IObservable<(float, Vector2)> OnZoom { get; }

        bool IsEnabled { get; }
        bool IsTapActive { get; }
        void SetEnabled(bool isEnabled);
    }
}