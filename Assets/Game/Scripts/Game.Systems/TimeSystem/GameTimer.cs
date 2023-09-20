using System;
using UniRx;
using Zenject;

namespace Game.Systems.TimeSystem
{
    public class GameTimer : ITimer, ITickable
    {
        public bool IsActive { get; private set; }
        public bool IsPaused { get; private set; }
        
        public ISubject<Unit> EventTimerFinish { get; private set; } = new Subject<Unit>();
        public IReactiveProperty<TimeSpan> Time { get; private set; } = new ReactiveProperty<TimeSpan>(TimeSpan.Zero);

        
        public void Start(TimeSpan timer)
        {
            if(IsActive) return;
            
            IsActive = true;

            Time.Value = timer;
        }

        public void Pause()
        {
            if(IsPaused || IsActive == false) return;

            IsPaused = true;
        }

        public void Continue()
        {
            if(IsPaused == false || IsActive == false) return;

            IsPaused = false;
        }

        public void Stop()
        {
            if(IsActive == false) return;

            IsActive = false;
            IsPaused = false;
        }
        
        public void Tick()
        {
            if(IsActive == false || IsPaused) return;

            Time.Value -= TimeSpan.FromSeconds(UnityEngine.Time.deltaTime);
            if (Time.Value.Ticks <= 0)
            {
                EventTimerFinish.OnNext(Unit.Default);
                Stop();
            }
        }
    }
}