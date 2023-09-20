using System;
using System.Collections.Generic;
using Game.Core.Configurations;
using Game.Data.Models;
using UniRx;
using Zenject;

namespace Game.Managers
{
    public class BoosterManager : IDisposable
    {
        private readonly GameDataModel gameDataModel;
        private readonly BoostersParameters boostersParameters;

        public BoosterManager(
            GameDataModel gameDataModel, 
            BoostersParameters boostersParameters)
        {
            this.gameDataModel = gameDataModel;
            this.boostersParameters = boostersParameters;
        }
        
        [Inject]
        private void Initialization()
        {
            gameDataModel.BoostersData ??= new List<BoostersData>();
            var boosters = Enum.GetValues(typeof(BoosterType)) as BoosterType[];
            foreach (var booster in boosters)
            {
                TryCreatBoosterData(booster);
            }
        }
        
        private void TryCreatBoosterData(BoosterType boosterType)
        {
            if (gameDataModel.BoostersData.Exists(t => t.BoosterType == boosterType) == false)
            {
                var initData = CreateBoosterData(boosterType);
                gameDataModel.BoostersData.Add(initData);
            }
        }

        private BoostersData CreateBoosterData(BoosterType boosterType)
        {
            var boosterGameSetting = boostersParameters.BoosterGameSettings.Find(t => t.Type == boosterType);
            var data = new BoostersData
            {
                BoosterType = boosterType,
                Counter = new IntReactiveProperty(boosterGameSetting.DefaultCount),
                OpenOnLevel = new IntReactiveProperty(boosterGameSetting.OpenOnLevel)
            };
            return data;
        }

        public void Dispose()
        {
        }
    }
}