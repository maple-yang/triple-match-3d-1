using UnityEngine;

namespace Game.Modules.TutorModule.View
{
    public class BaseTutorView : MonoBehaviour
    {
        [SerializeField] 
        protected ShadowRects shadowRects = null;
        
        [SerializeField] 
        protected TutorMessage[] tutorMessage = null;

        [SerializeField] 
        protected TutorFinger tutorFinger = null;
        
        [SerializeField] 
        protected TutorFinger tutorFinger3DRef = null;
        
        public ShadowRects ShadowRects => shadowRects;
        public TutorMessage[] TutorMessage => tutorMessage;
        public TutorFinger TutorFinger => tutorFinger;

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public TutorFinger CreateTutorFinger3D(Transform parent)
        {
            return Instantiate(tutorFinger3DRef, parent);
        }

        public void DestroyTutorFinger3D(TutorFinger tutorFinger3D)
        {
            Destroy(tutorFinger3D.gameObject);
        }
    }
}