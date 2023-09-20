using System;
using Game.Core.Components;
using Game.Data.Models;
using UniRx;

namespace Game.Core.Controllers
{
    public interface IBoostersController : IDisposable
    {
        void UseBooster(BoosterType boosterType, bool isFree);
        ISubject<BoosterType> EventBoosterActionStart { get; }
        ISubject<BoosterType> EventBoosterActionFinished { get; }
    }
}