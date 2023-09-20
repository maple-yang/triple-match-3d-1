using Cysharp.Threading.Tasks;
using Game.Modules.TutorModule.Data;
using Game.Modules.TutorModule.View;

namespace Game.Modules.TutorModule
{
    public interface ITutorLogic
    {
        public ITutorLogic CheckAvailabilityTutor(TutorLogicData tutorLogicData);
        public UniTask StartLogic(BaseTutorView tutorView, TutorLogicData tutorLogicData);
    }
}