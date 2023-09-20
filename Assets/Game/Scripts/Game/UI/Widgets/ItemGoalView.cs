using System;
using DG.Tweening;
using Game.Scripts.Game.Core.Configurations;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Game.UI.Widgets
{
    public class ItemGoalView : MonoBehaviour
    {
        [SerializeField]
        private Text goalCountText;
        
        [SerializeField]
        private Image goalImage;

        [SerializeField]
        private Image goalAchievedImage;
        
        [SerializeField]
        private Image goalBackImage;
        
        private GoalParameters goalParameters;
        private Sequence sequence;

        public void Show(Sprite goalSprite, GoalParameters goalParameters)
        {
            goalImage.sprite = goalSprite;
            this.goalParameters = goalParameters;
            
            ShowAnimation();
        }

        public void Hide()
        {
            TryKillSequence();
            sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.zero, goalParameters.HidingGoalDuration).SetEase(goalParameters.HidingGoalScaleEase))
                .OnKill(() =>
                {
                    Destroy(gameObject);
                });
        }

        private void ShowAnimation()
        {
            goalCountText.gameObject.SetActive(true);
            goalAchievedImage.gameObject.SetActive(false);
            goalBackImage.gameObject.SetActive(true);

            TryKillSequence();
            sequence = DOTween.Sequence();
            sequence.Append(transform.DORotate(Vector3.up * 90, goalParameters.RotatingGoalDuration / 2))
                .SetDelay(goalParameters.DelayBeforeRotateGoal)
                .AppendCallback(() =>
                {
                    goalBackImage.gameObject.SetActive(false);
                });
            sequence.Append(transform.DORotate(Vector3.zero, goalParameters.RotatingGoalDuration / 2));
        }
        
        public void ItemGoalAchieved()
        {
            goalCountText.gameObject.SetActive(false);
            goalAchievedImage.gameObject.SetActive(true);
        }

        public void UpdateItemCountView(int itemCount)
        {
            goalCountText.text = $"{itemCount}";
        }

        private void TryKillSequence()
        {
            if (sequence != null)
            {
                sequence.Kill();
                sequence = null;
            }
        }

        private void OnDestroy()
        {
            TryKillSequence();
        }
    }
}