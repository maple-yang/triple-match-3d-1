using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Core.Configurations
{
    [CreateAssetMenu(fileName = nameof(ResultPanelParameters), menuName = "Configs/GameCore/"+nameof(ResultPanelParameters), order = 0)]

    public class ResultPanelParameters : ScriptableObject
    {
        [field: SerializeField, TitleGroup("Animation settings")] 
        public Vector3 ShowStarsScale { get; private set; } = Vector3.one * 1.2f;
        
        [field: SerializeField, TitleGroup("Animation settings")]
        public float ShowStarsDuration { get; private set; } = 0.5f;
        
        [field: SerializeField, TitleGroup("Animation settings")]
        public float ShowStarsDelayBetween { get; private set; } = 0.1f;

        [field: SerializeField, TitleGroup("Animation settings")] 
        public AnimationCurve ShowStarsEase { get; private set; } = default;
        
        [field: SerializeField, TitleGroup("Game settings")] 
        public TimerParameter AddTimeForAds { get; private set; } = default;
    }
}