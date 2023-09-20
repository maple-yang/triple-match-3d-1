using System;

namespace Game.Core.Levels
{
    public interface ILevelLoader
    {
        Level CurrentLevel { get; }

        void LoadLevel(LevelData levelData, Action<Level> onLoaded = null);
        void UnloadCurrentLevel();
    }
}