using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Data.Models;
using Game.Modules.TutorModule;
using Game.Modules.TutorModule.Data;
using Game.Modules.TutorModule.View;
using Game.Tutor.Data;
using Game.UI.Components;
using UniRx;
using UnityEngine;

namespace Game.Tutor.Logics
{
    public class BoosterTutorLogic : ITutorLogic
    {
        [SerializeField] 
        private bool IsIgnoreAvailabilityTutor;
        
        [SerializeField] 
        private BoosterType boosterType;
        
        [SerializeField] 
        private Vector2 messagePositionOffset;

        private CompositeDisposable compositeDisposable;
        private bool isBoosterClick;

        public ITutorLogic CheckAvailabilityTutor(TutorLogicData tutorLogicData)
        {
#if UNITY_EDITOR
            if (IsIgnoreAvailabilityTutor)
            {
                return this;
            }
#endif
            var data = (GameTutorLogicData)tutorLogicData;
            var boosterData = data.GameDataModel.BoostersData.First(bd => bd.BoosterType == boosterType);
            if (boosterData.OpenOnLevel.Value == data.GameDataModel.LastOpenedLevelIndex + 1 &&
                !data.GameDataModel.CompletedTutorCache.Contains(ToString()))
            {
                return this;
            }

            return null;
        }

        public virtual async UniTask StartLogic(BaseTutorView tutorView, TutorLogicData tutorLogicData)
        {
            var data = (GameTutorLogicData)tutorLogicData;
            var boosterViews = data.GameLevel.LevelPanel.BoostersWidget.BoosterView;
            var boosterView = boosterViews.SingleOrDefault(t => t.BoosterType == boosterType);
            if (boosterView == null)
            {
                return;
            }

            await UniTask.Delay(200);
            var boosterRectTransform = boosterView.transform as RectTransform;
            tutorView.Show();
            tutorView.ShadowRects.Show();
            tutorView.ShadowRects.AddUnmask(boosterRectTransform, margin: Vector2.one * 5);
            tutorView.TutorMessage[0].Show(boosterRectTransform.position, messagePositionOffset);
            tutorView.TutorFinger.MoveAnimation(boosterRectTransform.position);

            isBoosterClick = false;
            compositeDisposable = new CompositeDisposable();
            boosterView.EventBoosterButton.Subscribe(OnBoosterClick).AddTo(compositeDisposable);

            await UniTask.WaitUntil(() => isBoosterClick);
            compositeDisposable.Dispose();
            tutorView.ShadowRects.RemoveAllUnmask();
            tutorView.ShadowRects.Hide();
            tutorView.TutorMessage[0].Hide();
            tutorView.TutorFinger.Hide();
            tutorView.Hide();

            await UniTask.Yield();
        }

        private void OnBoosterClick(BoosterButton boosterButton)
        {
            isBoosterClick = true;
        }

        public override string ToString()
        {
            return $"{GetType()}.{boosterType}";
        }
    }
}