using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data.Models;
using Game.Levels;
using UniRx;
using Zenject;

namespace Game.Providers.Goal
{
    public class GoalProvider : IGoalProvider, IDisposable
    {
        public GoalProvider(
            GameDataModel gameDataModel, 
            ILevelsProvider levelsProvider)
        {
            this.gameDataModel = gameDataModel;
            this.levelsProvider = levelsProvider;
        }

        private readonly GameDataModel gameDataModel;
        private readonly ILevelsProvider levelsProvider;

        private Dictionary<string, int> itemGoalDataDictionary = new Dictionary<string, int>();
        
        [Inject]
        private void Initialization()
        {
            var levelData = levelsProvider.GetLevelDataBySelectedLevelNumber();
            var itemParameters = levelData.LevelSettings.Items.Where(i => i.TargetCount > 0).ToList();
            for (int i = 0; i < itemParameters.Count; i++)
            {
                var itemParameter = itemParameters[i];
                itemGoalDataDictionary[itemParameter.ItemID] = itemParameter.TargetCount;
            }
            gameDataModel.RemainingItemGoalData = itemGoalDataDictionary;
        }

        public void Dispose()
        {
            gameDataModel.RemainingItemGoalData.Clear();
            itemGoalDataDictionary.Clear();
        }

        public Dictionary<string, int> GetRemainingItemGoalData()
        {
            return itemGoalDataDictionary;
        }

        public void UpdateItemGoalData(string itemId, int itemCount)
        {
            if (itemGoalDataDictionary.ContainsKey(itemId))
            {
                itemGoalDataDictionary[itemId] += itemCount;
            }

            if (itemGoalDataDictionary[itemId] <= 0)
            {
                itemGoalDataDictionary.Remove(itemId);
                if (itemGoalDataDictionary.Count <= 0)
                {
                    gameDataModel.EventLevelGoalCompleted.OnNext(Unit.Default);
                }
            }

            gameDataModel.RemainingItemGoalData = itemGoalDataDictionary;
        }
    }
}