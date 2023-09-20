using UniRx;

namespace Game.Modules.LivesManager.Model
{
    public interface ILivesController
    {
        ISubject<Unit> EventLevelStart { get; }
        ISubject<Unit> EventLevelLose { get; }
        ISubject<Unit> EventLevelCompleted { get; }
    }
}