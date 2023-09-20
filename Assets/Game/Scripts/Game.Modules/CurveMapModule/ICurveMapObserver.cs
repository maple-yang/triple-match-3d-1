using UniRx;
using UnityEngine;

namespace Game.Modules.CurveMapModule
{
    public interface ICurveMapObserver
    {
        ISubject<Vector2> EventCurveMapSwipe { get; set; }
        void SetActiveSwipe(bool isActive);
    }
}