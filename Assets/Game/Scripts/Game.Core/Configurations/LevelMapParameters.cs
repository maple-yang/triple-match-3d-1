using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Core.Configurations
{
    [CreateAssetMenu(fileName = nameof(LevelMapParameters), menuName = "Configs/GameCore/"+nameof(LevelMapParameters), order = 0)]

    public class LevelMapParameters : ScriptableObject
    {
        [field: Header("Level map widget")]
        [field: SerializeField]
        public Vector3 SelectedCellScale { get; private set; } = Vector3.one * 1.2f;
        
        [field: SerializeField]
        public float SelectedCellDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        public Ease SelectedCellEase { get; private set; } = Ease.Linear;
        
        [field: SerializeField]
        public int NumberClosedLevels { get; private set; } = 10;
        
        [field: Header("Adding stars animation")]
        [field: SerializeField] 
        public Image StarReference { get; private set; }
        
        [field: SerializeField]
        public float ShowStarsDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        public float ShowStarsDelayBetween { get; private set; } = 0.1f;

        [field: SerializeField] 
        public AnimationCurve ShowStarsEase { get; private set; } = default;
        
        
        [field: SerializeField, Space]
        public Vector3 MoveStarsScale { get; private set; } = Vector3.one * 0.6f;

        [field: SerializeField]
        public float MoveStarsDuration { get; private set; } = 1f;
        
        [field: SerializeField]
        public float MoveStarsDelayBetween { get; private set; } = 0.1f;
        
        [field: SerializeField]
        public Ease MoveStarsEase { get; private set; } = Ease.Linear;
        
        
        [field: SerializeField, Space]
        public float EndScaleStarsDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        public Ease EndScaleStarsEase { get; private set; } = Ease.Linear;
        
        [field: Header("Open line animation")]
        [field: SerializeField]
        public ParticleSystem OpenLineParticleSystem { get; private set; }
        
        [field: SerializeField]
        public float OpenLineDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        public Ease OpenLineEase { get; private set; } = Ease.Linear;
    }
}
