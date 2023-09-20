using System;
using UniRx;

namespace Game.Modules.LivesManager
{
    public interface ILivesProvider
    {
        void Initialization(LivesData livesData);
        void UpdateLivesView();
        void AddLives();
        ISubject<LivesData> EventLivesDataUpdated { get; }
        ISubject<bool> EventLivesCountFull { get; }
        ISubject<int> EventTemporaryLiveAction { get; }
        IReactiveProperty<TimeSpan> TimeRechargeLives { get; }
    }
}