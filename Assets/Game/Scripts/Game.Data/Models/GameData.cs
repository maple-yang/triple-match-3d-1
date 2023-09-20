using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.Serialization;

namespace Game.Data.Models
{
    [Serializable]
    public class GameData
    {
        public IntReactiveProperty LastCompletedLevelIndexReactive;
        public List<BoostersData> BoostersData;
        public List<LevelProgressData> LevelProgressData;
        public BoolReactiveProperty IsMusicReactive;
        public BoolReactiveProperty IsSoundReactive;
        public IntReactiveProperty LivesCountProperty;
        public int TimestampRechargeLives;
        public int TemporaryLive;
        public List<string> CompletedTutorCache;
        public List<RandomLevelData> RandomLevelDatas;
        public string Language;
        public bool IsFirstStartGame;
    }

    [Serializable]
    public class LevelProgressData
    {
        public int LevelIndex;
        public int NumberStars; 
    }
    
    [Serializable]
    public class RandomLevelData
    {
        public string LevelID;
        public List<int> TargetCount; 
    }
}