using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Game.Data.Models
{
    public class GameDataModel : DataModel<GameData>, IAutoBindDataModel
    {
        private IReadOnlyReactiveProperty<int> LastCompletedLevelIndexReactive => Data.LastCompletedLevelIndexReactive ??= new IntReactiveProperty(0);
        public IntReactiveProperty SelectedLevelIndexReactive { get; } = new IntReactiveProperty(-1);
        public Dictionary<string, int> RemainingItemGoalData { get; set; } = new Dictionary<string, int>();
        public ISubject<string> EventAddItemToField { get; private set; }
        public ISubject<string> EventRemoveItemFromField  { get; private set; }
        public ISubject<Unit> EventLevelGoalCompleted { get; private set; }
        public LevelResultData LevelResultData { get; set; }
        public bool IsCalculateGameFieldBounds { get; set; }
        public bool IsFirstStartGame
        {
            get => Data.IsFirstStartGame;
            set => Data.IsFirstStartGame = value;
        }

        public int LastOpenedLevelIndex
        {
            get => LastCompletedLevelIndexReactive.Value;
            set
            {
                if (value < 0)
                {
                    Debug.LogError($"You try set level {value}. Level index can't be negative!");
                    return;
                }
                
                Data.LastCompletedLevelIndexReactive.Value = value;
            }
        }

        public List<RandomLevelData> RandomLevelDatas => Data.RandomLevelDatas ??= new List<RandomLevelData>();

        protected override void OnDataLoaded()
        {
            base.OnDataLoaded();
            Data.LevelProgressData ??= new List<LevelProgressData>();
            EventAddItemToField = new Subject<string>();
            EventRemoveItemFromField = new Subject<string>();
            EventLevelGoalCompleted = new Subject<Unit>();
        }

#region Boosters

        public List<BoostersData> BoostersData
        {
            get => Data.BoostersData;
            set => Data.BoostersData = value;
        }

        public BoostersData GetBoosterData(BoosterType boosterType)
        {
            var data = BoostersData.SingleOrDefault(t => t.BoosterType == boosterType);
            if (data == null)
            {
                Debug.LogError($"There's no booster data for \'{boosterType}\'!");
                return null;
            }

            return data;
        }

#endregion
        
#region LevelMap

        public class ProgressStarsSelectedLevelData
        {
            public int Index = -1;
            public int Stars;
        }

        public ProgressStarsSelectedLevelData ProgressStarsSelectedLevel { get; set; } = new ProgressStarsSelectedLevelData();

        public int GetCurrentLevelProgressStars()
        {
            return GetNumberLevelProgressStars(SelectedLevelIndexReactive.Value);
        }
        
        public int GetNumberLevelProgressStars(int levelIndex)
        {
            var levelProgressData = Data.LevelProgressData.Find(li => li.LevelIndex == levelIndex);
            if (levelProgressData != null)
            {
                return levelProgressData.NumberStars;
            }
            return 0;
        }

        public void SetNumberLevelProgressStars(int levelIndex, int amountStars)
        {
            var levelProgressData = Data.LevelProgressData.Find(li => li.LevelIndex == levelIndex);
            if (levelProgressData == null)
            {
                levelProgressData = new LevelProgressData
                {
                    LevelIndex = levelIndex,
                    NumberStars = amountStars
                };
                Data.LevelProgressData.Add(levelProgressData);
                Save();
                return;
            }

            levelProgressData.NumberStars = amountStars;
            Save();
        }

#endregion

#region PausePanel

        public BoolReactiveProperty IsMusic
        {
            get => Data.IsMusicReactive ??= new BoolReactiveProperty(true);
            set
            {
                Data.IsMusicReactive = value;
                IsMusicChange.OnNext(Data.IsMusicReactive.Value);
                Save();
            }
        }
        
        public BoolReactiveProperty IsSound
        {
            get => Data.IsSoundReactive ??= new BoolReactiveProperty(true);
            set
            {
                Data.IsSoundReactive = value;
                IsSoundChange.OnNext(Data.IsSoundReactive.Value);
                Save();
            }
        }

        public ISubject<bool> IsMusicChange = new Subject<bool>();
        public ISubject<bool> IsSoundChange = new Subject<bool>();
        public string Language
        {
            get => Data.Language;
            set => Data.Language = value;
        }

        #endregion

#region Lives

        public IntReactiveProperty LivesCount
        {
            get => Data.LivesCountProperty ??= new IntReactiveProperty(-1);
            set => Data.LivesCountProperty = value;
        }
        
        public int TimestampRechargeLives
        {
            get => Data.TimestampRechargeLives;
            set => Data.TimestampRechargeLives = value;
        }
        
        public int TemporaryLive
        {
            get => Data.TemporaryLive;
            set => Data.TemporaryLive = value;
        }

#endregion

#region Tutor

        public List<string> CompletedTutorCache
        {
            get => Data.CompletedTutorCache = Data.CompletedTutorCache ?? new List<string>();
            set => Data.CompletedTutorCache = value;
        }

#endregion
    }
}