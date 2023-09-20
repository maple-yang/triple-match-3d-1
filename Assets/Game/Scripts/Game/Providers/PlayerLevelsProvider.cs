using System.Collections.Generic;
using System.Linq;
using Game.Configurations;
using Game.Core.Configurations;
using Game.Core.Levels;
using Game.Data.Models;
using Game.Utils.Utils;
using UnityEngine;

namespace Game.Providers
{
    public class PlayerLevelsProvider : ILevelsProvider
    {
        public PlayerLevelsProvider(GameDataModel gameDataModel, LevelsConfig levelsConfig, LevelMapParameters levelMapParameters)
        {
            this.gameDataModel = gameDataModel;
            this.levelsConfig = levelsConfig;
            this.levelMapParameters = levelMapParameters;
        }
        
        private readonly GameDataModel gameDataModel;
        private readonly LevelsConfig levelsConfig;
        private readonly LevelMapParameters levelMapParameters;
        private List<LevelData> levelDatas;
        private List<LevelData> randomLevelDatas;

        public Level Level { get; set; }

        public LevelData GetLevelDataBySelectedLevelNumber()
        {
            var selectedLevelIndex = gameDataModel.SelectedLevelIndexReactive.Value;
            // var levelsCount = levelsConfig.LevelsData.Count;
            // if (selectedLevelIndex >= levelsCount)
            // {
            //     var repeatLevels = GetRepeatLevels();
            //     var repeatIndex = (selectedLevelIndex - levelsCount) % repeatLevels.Count;
            //     var repeatLevelData = repeatLevels[repeatIndex];
            //     return repeatLevelData;
            // }

            SetLevelData();
            var levelData = levelDatas.ElementAt(selectedLevelIndex);
            return levelData;
        }

        public List<LevelData> GetListLevelData()
        {
            levelDatas = new List<LevelData>(levelsConfig.LevelsData.ToList());

            var numberClosedLevels = levelMapParameters.NumberClosedLevels;
            var lastCompletedLevelIndex = gameDataModel.LastOpenedLevelIndex;
            if (lastCompletedLevelIndex <= levelDatas.Count - numberClosedLevels)
            {
                return levelDatas;
            }

            var repeatLevels = GetRepeatLevels();
            for (int i = repeatLevels.Count; i < lastCompletedLevelIndex + numberClosedLevels; i++)
            {
                var repeatIndex = i % repeatLevels.Count;
                var repeatLevelData = repeatLevels[repeatIndex];
                levelDatas.Add(repeatLevelData);
            }

            return levelDatas;
        }
        
        public List<LevelData> GetSaveListLevelData(int needNumberLevels)
        {
            LoadRandomLevelData();
            SetLevelData();

            if (needNumberLevels <= levelDatas.Count)
            {
                return levelDatas;
            }

            CreateNewRandomLevelData();
            SetLevelData();
            
            return levelDatas;
        }

        private List<LevelData> GetRepeatLevels()
        {
            return levelsConfig.LevelsData.Where(t => t.IsRepeat).ToList();
        }

        private void SetLevelData()
        {
            levelDatas = new List<LevelData>();
            levelDatas.AddRange(levelsConfig.LevelsData.ToList());
            levelDatas.AddRange(randomLevelDatas);
        }

        private void LoadRandomLevelData()
        {
            if (randomLevelDatas != null)
            {
                return;
            }
            randomLevelDatas = new List<LevelData>();
            var saveLevelDatas = gameDataModel.RandomLevelDatas;
            for (int i = 0; i < saveLevelDatas.Count; i++)
            {
                var saveLevelData = saveLevelDatas[i];
                var levelData = levelsConfig.LevelsData.FirstOrDefault(ld => ld.LevelID == saveLevelData.LevelID);
                if (levelData != null)
                {
                    var randomLevelData = CreatNewLevelData(levelData, saveLevelData, i);
                    randomLevelDatas.Add(randomLevelData.Item1);
                }
            }
        }

        private void CreateNewRandomLevelData()
        {
            var repeatLevels = GetRepeatLevels();
            repeatLevels.Shuffle();
            var randomLevelCount = gameDataModel.RandomLevelDatas.Count;
            for (int i = 0; i < repeatLevels.Count; i++)
            {
                var repeatLevel = repeatLevels[i];
                var randomLevelData = CreatNewLevelData(repeatLevel, null,i + randomLevelCount);
                randomLevelDatas.Add(randomLevelData.Item1);
                gameDataModel.RandomLevelDatas.Add(randomLevelData.Item2);
            }
            gameDataModel.Save();
        }

        private (LevelData, RandomLevelData) CreatNewLevelData(LevelData levelData, RandomLevelData randomLevelData, int numberLevel)
        {
            var newLevelData = new LevelData
            {
                LevelID = levelData.LevelID + $"_save_data_{numberLevel}",
                LevelSettings = new LevelSettings
                {
                    Timer = levelData.LevelSettings.Timer,
                    ProgressStarts = levelData.LevelSettings.ProgressStarts,
                    Items = new List<ItemParameter>()
                }
            };
            foreach (var itemParameter in levelData.LevelSettings.Items)
            {
                var newItemParameters = new ItemParameter()
                {
                    ItemID = itemParameter.ItemID,
                    ItemsCount = itemParameter.ItemsCount,
                    TargetCount = itemParameter.TargetCount
                };
                newLevelData.LevelSettings.Items.Add(newItemParameters);
            }
            if (randomLevelData == null)
            {
                randomLevelData = new RandomLevelData
                {
                    LevelID = levelData.LevelID,
                    TargetCount = new List<int>()
                };
                var maxCountTarget = 5;
                for (int i = 0; i < newLevelData.LevelSettings.Items.Count; i++)
                {
                    var itemParameter = newLevelData.LevelSettings.Items[i];
                    var mergeCount = itemParameter.ItemsCount / 3;
                    var randomTargetCount = Random.Range(0, mergeCount);
                    var newTargetCount = randomTargetCount * 3;
                    newTargetCount = maxCountTarget == 0 ? 0 : newTargetCount;
                    maxCountTarget = newTargetCount > 0 ? maxCountTarget - 1 : maxCountTarget;

                    randomLevelData.TargetCount.Add(newTargetCount);
                    itemParameter.TargetCount = newTargetCount;
                }
                if (randomLevelData.TargetCount.All(tc => tc == 0))
                {
                    var maxItemCount = newLevelData.LevelSettings.Items.Max(i => i.ItemsCount);
                    var itemParameter = newLevelData.LevelSettings.Items.Find(i => i.ItemsCount == maxItemCount);
                    var index = newLevelData.LevelSettings.Items.IndexOf(itemParameter);
                    randomLevelData.TargetCount[index] = maxItemCount;
                    newLevelData.LevelSettings.Items.ForEach(i => i.TargetCount = 0);
                    itemParameter.TargetCount = maxItemCount;
                }
            }
            else
            {
                for (int i = 0; i < newLevelData.LevelSettings.Items.Count; i++)
                {
                    var itemParameter = newLevelData.LevelSettings.Items[i];
                    var targetCount = randomLevelData.TargetCount[i];
                    itemParameter.TargetCount = targetCount;
                }
            }
            return (newLevelData, randomLevelData);
        }
    }
}