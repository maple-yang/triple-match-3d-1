using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Modules.TutorModule.Data;
using Game.Modules.TutorModule.View;
using UnityEngine;

namespace Game.Modules.TutorModule.Model
{
    public class TutorProvider : MonoBehaviour, ITutorProvider
    {
        [SerializeField]
        private TutorParameters tutorParameters;
        
        private Dictionary<ITutorLogic, BaseTutorView> tutorViewDictionary = new Dictionary<ITutorLogic, BaseTutorView>();

        public ITutorLogic CheckAvailabilityTutor(TutorLogicData tutorLogicData)
        {
            foreach (var tutorData in tutorParameters.TutorDatas)
            {
                var tutorLogic = tutorData.TutorLogic.CheckAvailabilityTutor(tutorLogicData);
                if (tutorLogic != null)
                {
                    return tutorLogic;
                }
            }

            return null;
        }

        public async UniTask<BaseTutorView> LoadTutorViewAsync(ITutorLogic tutorLogic, TutorLogicData tutorLogicData)
        {
            var tutorStepView = tutorParameters.TutorDatas.FirstOrDefault(t => t.TutorLogic == tutorLogic);
            if (tutorStepView == null)
            {
                Debug.LogError($"{tutorLogic} does not exist");
                return null;
            }

            BaseTutorView baseTutorView = Instantiate(tutorStepView.TutorViewReference, tutorLogicData.ParentTransform);
            baseTutorView.Hide();
            Transform tutorStepViewTransform = baseTutorView.transform;
            tutorStepViewTransform.localScale = Vector3.one;
            tutorStepViewTransform.localPosition = Vector3.zero;
            tutorViewDictionary[tutorLogic] = baseTutorView;
            
            return baseTutorView;
        }

        public void UnloadTutorView(ITutorLogic tutorLogic)
        {
            if (tutorViewDictionary.TryGetValue(tutorLogic, out BaseTutorView baseTutorView))
            {
                tutorViewDictionary.Remove(tutorLogic);
                Destroy(baseTutorView.gameObject);
            }
        }
    }
}