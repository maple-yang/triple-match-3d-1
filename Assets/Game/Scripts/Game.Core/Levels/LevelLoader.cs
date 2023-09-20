using System;
using Cysharp.Threading.Tasks;
using Game.Core.Utils;
using UnityEngine;
using Zenject;

namespace Game.Core.Levels
{
    public class LevelLoader : MonoBehaviour, ILevelLoader
    {
        [SerializeField]
        private Transform levelsRoot;

        [SerializeField]
        private Level defaultLevel;
        
        [Inject]
        private readonly DiContainer diContainer;
        
        [Inject]
        private readonly IAssetProvider assetProvider;

        public Level CurrentLevel { get; private set; }
        
        public void LoadLevel(LevelData levelData, Action<Level> onLoaded = null)
        {
            if (CurrentLevel != null)
            {
                UnloadCurrentLevel();
            }

            if (levelData.UseCustomLevelInfo)
            {
                LoadLevelAsync(levelData, level =>
                {
                    CurrentLevel = level;
                    ShowLevel(levelData);
                    onLoaded?.Invoke(CurrentLevel);
                }).Forget();
            }
            else
            {
                CurrentLevel = defaultLevel;
                CurrentLevel.gameObject.SetActive(true);
                ShowLevel(levelData);
                onLoaded?.Invoke(CurrentLevel);
            }
        }

        public void UnloadCurrentLevel()
        {
            CurrentLevel.Hide();
            CurrentLevel.DeInitialize();

            if (CurrentLevel == defaultLevel)
            {
                CurrentLevel.gameObject.SetActive(false);
            }
            else
            {
                Destroy(CurrentLevel.gameObject);
            }

            CurrentLevel = null;
        }

        private void ShowLevel(LevelData levelData)
        {
            CurrentLevel.Initialize(levelData.LevelSettings);
            CurrentLevel.Show();
        }

        private async UniTask LoadLevelAsync(LevelData levelData, Action<Level> onLoaded)
        {
            var levelReference = levelData.LevelReference;
            var levelPrefab = await assetProvider.TryGetAsset<Level>(levelReference);
            var level = diContainer.InstantiatePrefabForComponent<Level>(levelPrefab, levelsRoot);
            onLoaded?.Invoke(level);
        }
    }
}