using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Core.Components;
using Game.Core.Configurations;
using Game.Core.Controllers;
using Game.Core.Levels;
using Game.Data.Models;
using Game.Modules.AudioManager;
using Game.Modules.CurveMapModule;
using Game.Modules.CurveMapModule.Model;
using Game.Providers;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Game.UI.Widgets
{
    public class LevelMapWidget : MonoBehaviour, ILevelMapProvider
    {
        [SerializeField]
        private CellLevelMapView cellLevelMapViewReference;
        
        [SerializeField] 
        private LevelMapScroll levelMapScroll = null;
        
        [Inject]
        private GameDataModel gameDataModel;
        
        [Inject]
        private LevelMapParameters levelMapParameters;
        
        [Inject]
        private ILevelsProvider levelsProvider;
        
        [Inject]
        private LevelMapProgressAnimator levelMapProgressAnimator;
        
        [Inject]
        private IAudioController audioController;

        private int currentScrollIndex;
        private LevelData selectedLevelData;
        private LevelMapData levelMapData;

        private Dictionary<CellLevelMapView, LevelData> openCellLevelMapViewDictionary = new Dictionary<CellLevelMapView, LevelData>();
        private Dictionary<CellLevelMapView, int> allCellLevelMapViewDictionary = new Dictionary<CellLevelMapView, int>();
        private List<LevelData> levelDatas = new List<LevelData>();

        public ISubject<bool> EventCurveMapActive { get; set; }

        public void Initialization()
        {
            EventCurveMapActive = new Subject<bool>();
        }

        public void Show(bool isPlaySound)
        {
            SetMapScroll();
            TryShowFlyingStarsAnimation();
            SetSelectedLevelIndex();
        }

        public void Hide()
        {
            foreach (var cellLevelMapView in allCellLevelMapViewDictionary)
            {
                cellLevelMapView.Key.Hide();
                cellLevelMapView.Key.EventCellLevelMapSelected -= OnEventCellLevelMapSelected;
                Destroy(cellLevelMapView.Key.gameObject);
            }
            openCellLevelMapViewDictionary.Clear();
            allCellLevelMapViewDictionary.Clear();
            levelDatas.Clear();
        }
        
        private void SetMapScroll()
        {
            levelMapData = new LevelMapData();
            levelMapData.LastOpenedLevelIndex = gameDataModel.LastOpenedLevelIndex + 1;
            levelMapData.SelectedLevelIndex = gameDataModel.LastOpenedLevelIndex;
            levelMapData.LevelCount = levelMapData.LastOpenedLevelIndex + levelMapParameters.NumberClosedLevels;

            levelDatas = levelsProvider.GetListLevelData();
            selectedLevelData = levelDatas[levelMapData.SelectedLevelIndex];
            
            levelMapScroll.EventSetupItem += OnEventSetupItem;
            levelMapScroll.EventHideItem += OnEventHideItem;
            levelMapScroll.Initialization(cellLevelMapViewReference, levelMapData.LevelCount, levelMapData.SelectedLevelIndex + 1);
        }

        public LevelData GetSelectedLevelData()
        {
            return selectedLevelData;
        }

        private void SetSelectedLevelIndex()
        {
            gameDataModel.SelectedLevelIndexReactive.Value = levelMapData.SelectedLevelIndex;
            var numberStars = gameDataModel.GetNumberLevelProgressStars(levelMapData.SelectedLevelIndex);
            gameDataModel.ProgressStarsSelectedLevel.Index = levelMapData.SelectedLevelIndex;
            gameDataModel.ProgressStarsSelectedLevel.Stars = numberStars;
        }

        private void OnEventCellLevelMapSelected(CellLevelMapView cellLevelMapView)
        {
            foreach (var cellLevelMap in openCellLevelMapViewDictionary)
            {
                cellLevelMap.Key.SetCellType(ECellMapState.Open);
            }
            cellLevelMapView.SetCellType(ECellMapState.Current);
            selectedLevelData = openCellLevelMapViewDictionary[cellLevelMapView];
            levelMapData.SelectedLevelIndex = allCellLevelMapViewDictionary[cellLevelMapView] - 1;
            SetSelectedLevelIndex();
        }

        private void OnEventSetupItem(Component component, int index)
        {
            index = levelMapData.LevelCount - index;
            var cellLevelMapView = (CellLevelMapView)component;
            allCellLevelMapViewDictionary[cellLevelMapView] = index;

            cellLevelMapView.Show(index, levelMapParameters);
            var cellLevelMapState = index == levelMapData.SelectedLevelIndex + 1
                ? ECellMapState.Current
                : index <= levelMapData.LastOpenedLevelIndex
                    ? ECellMapState.Open
                    : ECellMapState.Close;
            cellLevelMapView.SetCellType(cellLevelMapState);
            var lineLevelMapState = index >= levelMapData.LastOpenedLevelIndex
                ? ECellMapState.Close
                : ECellMapState.Open;
            cellLevelMapView.SetLineType(lineLevelMapState);
            cellLevelMapView.EventCellLevelMapSelected += OnEventCellLevelMapSelected;
            if (cellLevelMapState != ECellMapState.Close)
            {
                var levelData = levelDatas[index - 1];
                openCellLevelMapViewDictionary[cellLevelMapView] = levelData;
                int numberStars = gameDataModel.GetNumberLevelProgressStars(index - 1);
                cellLevelMapView.SetProgressStars(numberStars);
            }
            else
            {
                cellLevelMapView.SetProgressStars(0);
            }
            
            if(currentScrollIndex > index)
            {
                cellLevelMapView.transform.SetAsFirstSibling();
            }
            else
            {
                cellLevelMapView.transform.SetAsLastSibling();
            }

            currentScrollIndex = index;
        }
        
        private void OnEventHideItem(Component component, int index)
        {
            var cellLevelMapView = (CellLevelMapView)component;
            cellLevelMapView.Hide();
            cellLevelMapView.EventCellLevelMapSelected -= OnEventCellLevelMapSelected;
            openCellLevelMapViewDictionary.Remove(cellLevelMapView);
        }

        private void TryShowFlyingStarsAnimation()
        {
            var progressStarsSelectedLevel = gameDataModel.ProgressStarsSelectedLevel;
            if (progressStarsSelectedLevel.Index == -1)
            {
                return;
            }
            
            var currentNumberStars = gameDataModel.GetNumberLevelProgressStars(progressStarsSelectedLevel.Index);
            var oldNumberStars = progressStarsSelectedLevel.Stars;
            var numberStars = currentNumberStars - oldNumberStars;
            if (numberStars == 0)
            {
                return;
            }

            var cellLevelMapView = allCellLevelMapViewDictionary.FirstOrDefault(v => v.Value == progressStarsSelectedLevel.Index + 1).Key;
            if (cellLevelMapView == null)
            {
                levelMapScroll.SetActiveScrollRect(true);
                return;
            }
            
            levelMapScroll.SetActiveScrollRect(false);
            levelMapScroll.MoveScrollToIndex(progressStarsSelectedLevel.Index + 1);
            cellLevelMapView.SetLineType(ECellMapState.Close);
            
            var starsTransforms = new List<Transform>(cellLevelMapView.EmptyStarsTransforms);
            for (int i = 0; i < oldNumberStars; i++)
            {
                starsTransforms.RemoveAt(0);
            }
            cellLevelMapView.SetProgressStars(oldNumberStars);
            var isMovingMap = progressStarsSelectedLevel.Index == levelMapData.SelectedLevelIndex - 1;
            
            audioController.TryPlaySound(AudioNameData.ADDING_STARS_IN_MENU_SOUND);
            levelMapProgressAnimator.ShowFlyingStarsAnimation(starsTransforms, numberStars, () =>
            {
                cellLevelMapView.SetProgressStars(currentNumberStars);
                if (isMovingMap)
                {
                    MovingMapAnimation(cellLevelMapView);
                }
                else
                {
                    levelMapScroll.SetActiveScrollRect(true);
                }
            }, () =>
            {
                audioController.TryPlaySound(AudioNameData.FLIGHT_OF_STARS_TO_STARS);
            });
        }

        private void MovingMapAnimation(CellLevelMapView cellLevelMapView)
        {
            levelMapScroll.MoveScrollFromIndexToIndex(levelMapData.SelectedLevelIndex, levelMapData.SelectedLevelIndex + 1);
            cellLevelMapView.OpenLineAnimation(() =>
            {
                levelMapScroll.SetActiveScrollRect(true);
            });

        }
    }
}