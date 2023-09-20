using Cysharp.Threading.Tasks;
using Game.Data.Models;
using Game.UI.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Loader
{
    public class GameLoader : MonoBehaviour
    {
        [SerializeField]
        private GameLoadingPanel loadingPanel;

        [Inject]
        private GameDataModel gameDataModel;

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            await ShowSplashScreen();
            loadingPanel.SetProgress(0.33f);

            await LoadApp();
            loadingPanel.SetProgress(0.66f);

            await LoadNexScene();
            loadingPanel.SetProgress(1f);

            var unloadTask = UnloadStartScene();
            var hidePanelTask = HideLoading();
            await UniTask.WhenAll(unloadTask, hidePanelTask);
            
            Destroy(gameObject);
        }

        private async UniTask ShowSplashScreen()
        {
            loadingPanel.Initialize();
            loadingPanel.Show(true);
            await UniTask.Yield();
        }

        private async UniTask LoadApp()
        {
            // initialization callbacks
            var loadCommand = gameDataModel.Load();
            var dataTask = UniTask.WaitWhile(() => loadCommand.IsCompleted == false);
            await UniTask.WhenAll(dataTask);
        }

        private async UniTask LoadNexScene()
        {
            var loadingScene = SceneManager.GetSceneByBuildIndex(0);

            if (loadingScene.isLoaded)
            {
                if (SceneManager.sceneCountInBuildSettings == 1)
                {
                    await UniTask.Yield();
                    return;
                }
                
                const int nextSceneIndex = 1;
                var nextScene = GetSceneByIndex(nextSceneIndex);

                if (nextScene.IsValid())
                {
                    await UniTask.Yield();
                }
                else
                {
                    await SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Additive).ToUniTask();
                }
            }
            else
            {
                await UniTask.Yield();
            }
        }

        private Scene GetSceneByIndex(int sceneIndex)
        {
            Scene nextScene = default;

            try
            {
                nextScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            }
            catch
            {
                // ignored
            }

            return nextScene;
        }

        private async UniTask UnloadStartScene()
        {
            var loadingScene = SceneManager.GetSceneByBuildIndex(0);
            await SceneManager.UnloadSceneAsync(loadingScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).ToUniTask();
        }

        private async UniTask HideLoading()
        {
            loadingPanel.DeInitialize();
            loadingPanel.Hide(instant: true);
            await UniTask.WaitWhile(() => loadingPanel.IsActive);
        }
    }
}