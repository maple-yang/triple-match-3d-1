using UniRx;

namespace Game.Core.Controllers
{
    public interface ILevelMapObserver
    {
        public ISubject<bool> EventLevelMapActive { get; set; }
    }
}