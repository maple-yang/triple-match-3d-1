using System;
using UniRx;
using Zenject;

namespace Game.Modules.LivesManager.Model
{
    public class LivesManager : ILivesProvider, ILivesController, IDisposable, ITickable
    {
        public LivesManager(LivesParameters livesParameters)
        {
            this.livesParameters = livesParameters;
        }
        
        private CompositeDisposable compositeDisposable;
        private LivesParameters livesParameters;
        private LivesData livesData;
        private bool isActive;
        private int removeLives;
        private int currentTimestamp => (int)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        
        public ISubject<Unit> EventLevelStart { get; private set; }
        public ISubject<Unit> EventLevelLose { get; private set; }
        public ISubject<Unit> EventLevelCompleted { get; private set; }
        public ISubject<LivesData> EventLivesDataUpdated { get; private set; }
        public ISubject<bool> EventLivesCountFull { get; private set; }
        public ISubject<int> EventTemporaryLiveAction { get; private set; }
        public IReactiveProperty<TimeSpan> TimeRechargeLives { get; private set; }

        public void Initialization(LivesData livesData)
        {
            this.livesData = livesData;

            EventLevelStart = new Subject<Unit>();
            EventLevelLose = new Subject<Unit>();
            EventLevelCompleted = new Subject<Unit>();
            EventLivesDataUpdated = new Subject<LivesData>();
            EventLivesCountFull = new Subject<bool>();
            EventTemporaryLiveAction = new Subject<int>();
            TimeRechargeLives = new ReactiveProperty<TimeSpan>(TimeSpan.Zero);
            compositeDisposable = new CompositeDisposable();
            
            EventLevelStart.Subscribe(LevelStart).AddTo(compositeDisposable);
            EventLevelLose.Subscribe(LevelLose).AddTo(compositeDisposable);
            EventLevelCompleted.Subscribe(LevelCompleted).AddTo(compositeDisposable);
            
            CalculateRechargingTimeToLives();
            TryRechargingLives();
            UpdateLivesData();
        }

        public void UpdateLivesView()
        {
            EventLivesCountFull.OnNext(!isActive);
            UpdateLivesData();
        }
        
        public void AddLives()
        {
            livesData.LivesCount += livesParameters.NumberLivesForAds;
            UpdateLivesData();
            if (livesData.LivesCount >= livesParameters.MaxLevelCount)
            {
                livesData.TimestampRechargeLives = 0;
                isActive = false;
                EventLivesCountFull.OnNext(!isActive);
            }
        }

        private void CalculateRechargingTimeToLives()
        {
            livesData.LivesCount = livesData.LivesCount < 0 ? livesParameters.DefaultStartLevelCount : livesData.LivesCount;
            livesData.LivesCount -= livesData.TemporaryLive;
            if (livesData.LivesCount >= livesParameters.MaxLevelCount || livesData.TimestampRechargeLives == 0)
            {
                return;
            }
            
            var livesRechargeInSeconds = livesData.TimestampRechargeLives - currentTimestamp;
            if (livesRechargeInSeconds > 0)
            {
                return;
            }
            
            double l = Math.Abs((double)livesRechargeInSeconds) / livesParameters.LivesRechargeInSeconds;
            int lives = (int)Math.Ceiling(l);
            if (livesData.LivesCount + lives >= livesParameters.MaxLevelCount)
            {
                livesData.LivesCount = livesParameters.MaxLevelCount;
                livesData.TimestampRechargeLives = 0;
                return;
            }
                
            livesData.LivesCount += lives;
        }

        private void TryRechargingLives()
        {
            if (isActive)
            {
                return;
            }

            if (livesData.LivesCount < livesParameters.MaxLevelCount)
            {
                SetLivesRechargeInSeconds();
                isActive = true;
            }
            else
            {
                livesData.TimestampRechargeLives = 0;
                isActive = false;
            }
            EventLivesCountFull.OnNext(!isActive);
        }
        
        private void SetLivesRechargeInSeconds()
        {
            if (livesData.TimestampRechargeLives == 0)
            { 
                livesData.TimestampRechargeLives = currentTimestamp + livesParameters.LivesRechargeInSeconds;
                return;
            }

            if (livesData.TimestampRechargeLives > currentTimestamp)
            {
                return;
            }
            
            var livesRechargeInSeconds = livesData.TimestampRechargeLives - currentTimestamp;
            var remainingTime = Math.Abs(livesRechargeInSeconds) % livesParameters.LivesRechargeInSeconds;
            livesData.TimestampRechargeLives = remainingTime + currentTimestamp;
        }
        
        public void Tick()
        {
            if (isActive == false)
            {
                return;
            }

            TimeRechargeLives.Value = TimeSpan.FromSeconds(livesData.TimestampRechargeLives - currentTimestamp);
            if (TimeRechargeLives.Value.Ticks <= 0)
            {
                livesData.LivesCount++;
                livesData.TimestampRechargeLives = 0;
                isActive = false;
                TryRechargingLives();
                UpdateLivesData();
            }
        }
        
        private void UpdateLivesData()
        {
            EventLivesDataUpdated.OnNext(livesData);
        }

        private void LevelStart(Unit unit)
        {
            removeLives = 1;
            EventTemporaryLiveAction.OnNext(removeLives);
        }

        private void LevelLose(Unit unit)
        {
            RemoveLives();
            TryRechargingLives();
            UpdateLivesData();
        }

        private void LevelCompleted(Unit unit)
        {
            removeLives = 0;
            EventTemporaryLiveAction.OnNext(removeLives);
        }

        private void RemoveLives()
        {
            livesData.LivesCount -= removeLives;
            removeLives = 0;
            EventTemporaryLiveAction.OnNext(removeLives);
        }
        
        public void Dispose()
        {
            RemoveLives();
            SetLivesRechargeInSeconds();
            UpdateLivesData();
            compositeDisposable.Dispose();
            compositeDisposable.Clear();
        }
    }
}