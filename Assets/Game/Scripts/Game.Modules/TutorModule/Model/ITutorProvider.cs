using Cysharp.Threading.Tasks;
using Game.Modules.TutorModule.Data;
using Game.Modules.TutorModule.View;

namespace Game.Modules.TutorModule.Model
{
    public interface ITutorProvider
    {
        public ITutorLogic CheckAvailabilityTutor(TutorLogicData tutorLogicData);
        public UniTask<BaseTutorView> LoadTutorViewAsync(ITutorLogic tutorLogic, TutorLogicData tutorLogicData);
        public void UnloadTutorView(ITutorLogic tutorLogic);
    }
}