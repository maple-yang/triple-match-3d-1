using Cysharp.Threading.Tasks;
using Game.Modules.TutorModule.Data;
using UniRx;
using Zenject;

namespace Game.Modules.TutorModule.Model
{
    public class TutorController : ITutorController
    {
        public TutorController(ITutorProvider tutorProvider)
        {
            this.tutorProvider = tutorProvider;
        }

        private readonly ITutorProvider tutorProvider;
        
        public ISubject<ITutorLogic> EventTutorStepShow { get; private set; }
        public ISubject<ITutorLogic> EventTutorStepHide { get; private set; }
        
        [Inject]
        private void Initialization()
        {
            EventTutorStepShow = new Subject<ITutorLogic>();
            EventTutorStepHide = new Subject<ITutorLogic>();
        }
        
        public ITutorLogic CheckAvailabilityTutor(TutorLogicData tutorLogicData)
        {
            return tutorProvider.CheckAvailabilityTutor(tutorLogicData);
        }

        public async UniTask ShowTutor(ITutorLogic tutorLogic, TutorLogicData tutorLogicData)
        {
            var tutorStepView = await tutorProvider.LoadTutorViewAsync(tutorLogic, tutorLogicData);
            if (tutorStepView == null)
            {
                return;
            }

            var tutorFinishedTask = tutorLogic.StartLogic(tutorStepView, tutorLogicData);
            EventTutorStepShow.OnNext(tutorLogic);
            await UniTask.WhenAll(tutorFinishedTask);
            
            tutorProvider.UnloadTutorView(tutorLogic);
            EventTutorStepHide.OnNext(tutorLogic);
        }
    }
}