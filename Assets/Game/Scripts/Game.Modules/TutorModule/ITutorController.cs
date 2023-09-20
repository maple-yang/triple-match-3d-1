using Cysharp.Threading.Tasks;
using Game.Modules.TutorModule.Data;
using UniRx;

namespace Game.Modules.TutorModule
{
    public interface ITutorController
    {
        public ISubject<ITutorLogic> EventTutorStepShow { get; }
        public ISubject<ITutorLogic> EventTutorStepHide { get; }
        public ITutorLogic CheckAvailabilityTutor(TutorLogicData tutorLogicData);
        public UniTask ShowTutor(ITutorLogic tutorLogic, TutorLogicData tutorLogicData);
    }
}