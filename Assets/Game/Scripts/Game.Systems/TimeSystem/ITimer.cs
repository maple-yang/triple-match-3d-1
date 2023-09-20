using System;
using UniRx;

namespace Game.Systems.TimeSystem
{
    public interface ITimer
    {
        bool IsActive { get; }
        bool IsPaused { get; }
        
        ISubject<Unit> EventTimerFinish { get; }
        IReactiveProperty<TimeSpan> Time { get; }

        void Start(TimeSpan timer);
        void Pause();
        void Continue();
        void Stop();
    }
}