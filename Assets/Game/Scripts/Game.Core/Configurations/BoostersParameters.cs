using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Data.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Core.Configurations
{
    [CreateAssetMenu(fileName = nameof(BoostersParameters), menuName = "Configs/GameCore/"+nameof(BoostersParameters), order = 0)]
    public class BoostersParameters : ScriptableObject
    {
        [field: SerializeField, TitleGroup("Cancel booster"), Header("Animation settings")]
        public float JumpDuration { get; private set; } = 0.5f;

        [field: SerializeField, TitleGroup("Cancel booster")]
        public float JumpHeight { get; private set; } = 1f;

        [field: SerializeField, TitleGroup("Cancel booster")]
        public Ease JumpEase { get; private set; } = Ease.Linear;

        
        [field: SerializeField, TitleGroup("Shuffle booster")]
        public float ShuffleForceMin { get; private set; } = 10f;
        
        [field: SerializeField, TitleGroup("Shuffle booster")]
        public float ShuffleForceMax { get; private set; } = 15f;

        [field: SerializeField, TitleGroup("Shuffle booster")]
        public float ShuffleAngularForceMin { get; private set; } = -100f;
        
        [field: SerializeField, TitleGroup("Shuffle booster")]
        public float ShuffleAngularForceMax { get; private set; } = 100f;

        [field: SerializeField, TitleGroup("Shuffle booster")]
        public float ShuffleCooldown { get; private set; } = 3f;
        
        
        [field: SerializeField, TitleGroup("Freezing booster")]
        public float FreezingDuration { get; private set; } = 3f;


        [field: SerializeField, Header("Game settings")]
        public int AddBoosterForAds { get; private set; } = 2;
        
        [field: SerializeField, Header("Game settings")]
        public List<BoosterGameSetting> BoosterGameSettings { get; private set; } = new List<BoosterGameSetting>();
        
        [Serializable]
        public class BoosterGameSetting
        {
            public BoosterType Type;
            public int DefaultCount = 1;
            public int OpenOnLevel = 1;
        }
    }
}