using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Core.Components;
using Game.Managers;
using Game.Modules.TutorModule;
using Game.Modules.TutorModule.Data;
using Game.Modules.TutorModule.View;
using Game.Tutor.Data;
using UniRx;
using UnityEngine;

namespace Game.Tutor.Logics
{
    public class FirstLevelTutorLogic : ITutorLogic
    {
        [Serializable]
        public class MessageData
        {
            public Vector2 OffsetPosition = Vector2.zero;
        }

        [SerializeField] 
        private bool IsIgnoreAvailabilityTutor;
        
        [SerializeField] 
        private float TimeShowingSecondMessage = 3;
        
        [SerializeField]
        private MessageData[] messageDatas = new MessageData[2];
        
        private List<ItemView> targetItemViews;
        private bool isItemClick;

        public ITutorLogic CheckAvailabilityTutor(TutorLogicData tutorLogicData)
        {
#if UNITY_EDITOR
            if (IsIgnoreAvailabilityTutor)
            {
                return this;
            }
#endif
            var data = (GameTutorLogicData)tutorLogicData;
            if (data.GameDataModel.LastOpenedLevelIndex == 0 && !data.GameDataModel.CompletedTutorCache.Contains(ToString()))
            {
                return this;
            }

            return null;
        }
 
        public async UniTask StartLogic(BaseTutorView tutorView, TutorLogicData tutorLogicData)
        {
            var data = (GameTutorLogicData)tutorLogicData;
            var itemViews = data.GameLevel.ItemsContainer.Items.ToList();
            var firstGoalItemId = data.GameDataModel.RemainingItemGoalData.First().Key;
            targetItemViews = itemViews.Where(i => i.ID == firstGoalItemId).ToList();

            var levelBounds = data.GameLevel.ItemsContainer.LevelBounds;
            for (int i = 0; i < itemViews.Count; i++)
            {
                var item = itemViews[i];
                item.SetInteractable(false);
                item.SetActivePhysics(false);
                item.Rotation = Quaternion.Euler(Vector3.zero);
                
                var offsetX = 0.5f;
                var offsetZ = levelBounds.size.z > 3 ? 0.5f : 0;
                var offsetZMax = levelBounds.size.z > 3 ? 0 : 1;
                var tx = (i % 3) / 2f;
                var x = Mathf.Lerp(levelBounds.min.x + offsetX, levelBounds.max.x - offsetX, tx);
                var tz = (int)(i / 3) / 2f;
                var z = Mathf.Lerp(levelBounds.max.z - offsetZ + offsetZMax, levelBounds.min.z + offsetZ, tz);
                var y = 0.5f;
                item.Position = new Vector3(x, y, z);
            }
            
            tutorView.Show();
            tutorView.ShadowRects.Hide();
            tutorView.TutorFinger.Hide();
            var boosterTransform = data.GameLevel.LevelPanel.BoostersWidget.transform;
            var messagePosition = new Vector3(0, boosterTransform.position.y, boosterTransform.position.z);
            ShowMessage(0, tutorView, messagePosition, messageDatas[0]);

            await UniTask.Delay(500);

            var tutorFinger3D = tutorView.CreateTutorFinger3D(data.GameLevel.transform);
            for (int i = 0, count = targetItemViews.Count; i < count; i++)
            {
                var targetItemView = targetItemViews[0];
                await TargetItemView(tutorFinger3D, targetItemView);
            }
            
            tutorView.DestroyTutorFinger3D(tutorFinger3D);
            itemViews = data.GameLevel.ItemsContainer.Items.ToList();
            itemViews.ForEach(i => i.SetInteractable(true));
            
            var itemGoalView = data.GameLevel.LevelPanel.GoalWidget.ItemGoalViewDictionary.First().Value;
            ShowMessage(1, tutorView, itemGoalView.transform.position, messageDatas[1]);
            await UniTask.Delay((int)(TimeShowingSecondMessage * 1000));

            tutorView.TutorMessage[0].Hide();
            tutorView.TutorMessage[1].Hide();
            tutorView.Hide();

            await UniTask.Yield();
        }

        private async UniTask TargetItemView(TutorFinger tutorFinger, ItemView targetItemView)
        {
            tutorFinger.Move3DAnimation(targetItemView.transform);

            isItemClick = false;
            var compositeDisposable = new CompositeDisposable();
            targetItemView.EventItemClick.Subscribe(OnItemClick).AddTo(compositeDisposable);
            targetItemView.SetInteractable(true);
            await UniTask.WaitUntil(() => isItemClick);
            compositeDisposable.Dispose();
            targetItemViews.Remove(targetItemView);

            await UniTask.Yield();
        }
        
        private void OnItemClick(ItemView itemView)
        {
            isItemClick = true;
        }

        private void ShowMessage(int numberMessage, BaseTutorView tutorView, Vector3 position, MessageData messageData)
        {
            foreach (var tutorMessage in tutorView.TutorMessage)
            {
                tutorMessage.Hide();
            }
            tutorView.TutorMessage[numberMessage].Show(position, messageData.OffsetPosition);
        }
    }
}