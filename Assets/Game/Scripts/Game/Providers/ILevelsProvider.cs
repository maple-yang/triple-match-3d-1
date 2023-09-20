using System.Collections.Generic;
using Game.Core.Levels;
using Game.Levels;

namespace Game.Providers
{
    public interface ILevelsProvider
    {
        Level Level { get; set; }
        LevelData GetLevelDataBySelectedLevelNumber();
        List<LevelData> GetListLevelData();
        List<LevelData> GetSaveListLevelData(int needNumberLevels);
    }
}