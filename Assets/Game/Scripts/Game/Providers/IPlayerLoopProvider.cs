using Game.Data;
using UniRx;

namespace Game.Providers
{
    public interface IPlayerLoopProvider
    {
        ISubject<Unit> EventGameLoaded { get; }
        ISubject<Unit> EventLevelStart { get; }
        ISubject<Unit> EventPreLevelLose { get; }
        ISubject<Unit> EventLevelLose { get; }
        ISubject<Unit> EventLevelCompleted { get; }
        ISubject<ELevelResultState> EventLevelContinue { get; }
    }
}