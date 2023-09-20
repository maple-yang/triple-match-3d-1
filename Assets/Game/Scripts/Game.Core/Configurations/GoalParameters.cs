using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Game.Core.Configurations
{
    [CreateAssetMenu(fileName = nameof(GoalParameters), menuName = "Configs/GameCore/"+nameof(GoalParameters), order = 0)]
    public class GoalParameters : ScriptableObject
    {
        [field: SerializeField]
        public float DelayBeforeHidingGoal { get; private set; } = 0.5f;
        
        [field: SerializeField]
        public float HidingGoalDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        public Ease HidingGoalScaleEase { get; private set; } = Ease.Linear;
        
        [field: SerializeField]
        public float RotatingGoalDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        public float DelayBeforeRotateGoal { get; private set; } = 0.5f;
    }
}
