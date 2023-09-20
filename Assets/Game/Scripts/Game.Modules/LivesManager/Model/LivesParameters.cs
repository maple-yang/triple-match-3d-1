using UnityEngine;

namespace Game.Modules.LivesManager.Model
{
    [CreateAssetMenu(fileName = nameof(LivesParameters), menuName = "Configs/Modules/"+nameof(LivesParameters), order = 0)]
    public class LivesParameters : ScriptableObject
    {
        [field: SerializeField] 
        public int DefaultStartLevelCount { get; private set; } = 5;
        
        [field: SerializeField] 
        public int MaxLevelCount { get; private set; } = 5;
        
        [field: SerializeField] 
        public int LivesRechargeInSeconds { get; private set; } = 10;
        
        [field: SerializeField] 
        public int NumberLivesForAds { get; private set; } = 1;
    }
}