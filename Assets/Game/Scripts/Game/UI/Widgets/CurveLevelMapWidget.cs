using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core.Controllers;
using Game.Core.Levels;
using Game.Data.Models;
using Game.Modules.AudioManager;
using Game.Providers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Modules.CurveMapModule.Model
{
    public class CurveLevelMapWidget : MonoBehaviour, ILevelMapProvider
    {
        [SerializeField] 
        private Transform mapParent;
        
        [SerializeField] 
        private Button upMapButton;
        
        [SerializeField] 
        private Button downMapButton;
        
        [SerializeField]
        private CurveMapParameters curveMapParameters;
        
        [Inject]
        private GameDataModel gameDataModel;
        
        [Inject]
        private ILevelsProvider levelsProvider;
        
        [Inject]
        private LevelMapProgressAnimator levelMapProgressAnimator;
        
        [Inject]
        private IAudioController audioController;
        
        [Inject]
        private ICurveMapObserver curveMapObserver;

        private LevelData selectedLevelData;
        private LevelMapData data;
        private CompositeDisposable compositeDisposable;

        private ComponentPool<byte, CurveCellMapView> cellMapViewPool;
        private ComponentPool<byte, CurveMapView> mapViewPool;
        private SortedList<int, CurveMapView> mapViewPoolSortedList;
        private Dictionary<CurveMapView, List<CurveCellMapView>> mapViewDictionary;
        private Dictionary<CurveCellMapView, LevelData> openCellMapViewDictionary;
        private Dictionary<CurveCellMapView, int> allCellMapViewDictionary;
        private List<LevelData> levelDatas;

        public ISubject<bool> EventCurveMapActive { get; set; }

        public void Initialization()
        {
            compositeDisposable = new CompositeDisposable();
            EventCurveMapActive = new Subject<bool>();
            data = new LevelMapData();
            cellMapViewPool = new ComponentPool<byte, CurveCellMapView>();
            mapViewPool = new ComponentPool<byte, CurveMapView>();
            mapViewPoolSortedList = new SortedList<int, CurveMapView>();
            mapViewDictionary = new Dictionary<CurveMapView, List<CurveCellMapView>>();
            openCellMapViewDictionary = new Dictionary<CurveCellMapView, LevelData>();
            allCellMapViewDictionary = new Dictionary<CurveCellMapView, int>();
            levelDatas = new List<LevelData>();
        }

        public void Show(bool isPlaySound)
        {
            SetupMapData();
            SetupMap(0);
            FlyingStarsAsync(isPlaySound);
            SetSelectedLevelIndex();
        }

        public void Hide()
        {
            foreach (var cellMapView in allCellMapViewDictionary)
            {
                cellMapView.Key.Hide();
                cellMapView.Key.EventCellLevelMapSelected -= OnEventCellLevelMapSelected;
                Destroy(cellMapView.Key.gameObject);
            }
            openCellMapViewDictionary.Clear();
            allCellMapViewDictionary.Clear();
            levelDatas.Clear();
            compositeDisposable.Dispose();
            compositeDisposable.Clear();
            
            upMapButton.onClick.RemoveListener(OnButtonUpMap);
            downMapButton.onClick.RemoveListener(OnButtonDownMap);
        }
        
        private void SetupMapData()
        {
            data.LastOpenedLevelIndex = gameDataModel.LastOpenedLevelIndex + 1;
            data.SelectedLevelIndex = gameDataModel.LastOpenedLevelIndex;
            data.CurrentTotalMapIndex = GetCurrentTotalMapIndex();
            data.LastTotalOpenedMapIndex = data.CurrentTotalMapIndex;
            data.MapCount = data.LastTotalOpenedMapIndex + 1;

            var needNumberLevels = GetNumberLevelByMapIndex(data.MapCount + 1);
            levelDatas = levelsProvider.GetSaveListLevelData(needNumberLevels);
            selectedLevelData = levelDatas[data.SelectedLevelIndex];

            upMapButton.onClick.AddListener(OnButtonUpMap);
            downMapButton.onClick.AddListener(OnButtonDownMap);
            curveMapObserver.EventCurveMapSwipe.Subscribe(OnSwipeMap).AddTo(compositeDisposable);
        }
        
        private int GetCurrentTotalMapIndex()
        {
            var levelCount = 0;
            var sumMapLevels = curveMapParameters.MapDatas.Sum(m => m.CellAnchors.Count);
            var x = (data.LastOpenedLevelIndex - 1) / sumMapLevels;
            var levelIndex = (data.LastOpenedLevelIndex - 1) - (sumMapLevels * x);
            int mapCount = x * curveMapParameters.MapDatas.Count;
            
            for (int i = 0; i < curveMapParameters.MapDatas.Count; i++)
            {
                var mapData = curveMapParameters.MapDatas[i];
                levelCount += mapData.CellAnchors.Count;
                if (levelCount > levelIndex)
                {
                    break;
                }

                var isHaveNewStars = CheckProgressStarsSelectedLevel();
                if (levelCount == levelIndex && isHaveNewStars)
                {
                    break;
                }
                mapCount++;
            }


            return mapCount;
        }

        private int GetNumberLevelByMapIndex(int mapIndex)
        {
            var numberLevels = 0;
            for (int i = 0; i < mapIndex; i++)
            {
                var mapData = curveMapParameters.MapDatas[i % curveMapParameters.MapDatas.Count];
                numberLevels += mapData.CellAnchors.Count;
            }

            return numberLevels;
        }

        public LevelData GetSelectedLevelData()
        {
            return selectedLevelData;
        }

        private void SetSelectedLevelIndex()
        {
            gameDataModel.SelectedLevelIndexReactive.Value = data.SelectedLevelIndex;
            var numberStars = gameDataModel.GetNumberLevelProgressStars(data.SelectedLevelIndex);
            gameDataModel.ProgressStarsSelectedLevel.Index = data.SelectedLevelIndex;
            gameDataModel.ProgressStarsSelectedLevel.Stars = numberStars;
        }

        private void OnEventCellLevelMapSelected(CurveCellMapView curveCellMapView)
        {
            foreach (var cellLevelMap in openCellMapViewDictionary)
            {
                cellLevelMap.Key.SetCellType(ECellMapState.Open);
            }
            curveCellMapView.SetCellType(ECellMapState.Current);
            selectedLevelData = openCellMapViewDictionary[curveCellMapView];
            data.SelectedLevelIndex = allCellMapViewDictionary[curveCellMapView] - 1;
            SetSelectedLevelIndex();
            audioController.TryPlaySound(AudioNameData.CLICK_SOUND);
        }

        private void OnSwipeMap(Vector2 swipeDirection)
        {
            var direction = Mathf.RoundToInt(-swipeDirection.y);
            SwitchMap(direction).Forget();
        }

        private void OnButtonUpMap()
        {
            SwitchMap(1).Forget();
        }

        private void OnButtonDownMap()
        {
            SwitchMap(-1).Forget();
        }

        private async UniTask SwitchMap(int direction)
        {
            if ((data.CurrentTotalMapIndex <= 0 && direction == -1) || 
                (data.CurrentTotalMapIndex >= data.MapCount && direction == 1) ||
                direction == 0)
            {
                return;
            }

            EventCurveMapActive.OnNext(true);
            SetActiveSwitchMap(false);
            SetupMap(direction);
            await MoveMapAsync(direction);
            HideMap(direction);
            SetActiveSwitchMap(true);
            EventCurveMapActive.OnNext(false);
        }

        private void SetupMap(int direction)
        {
            data.CurrentTotalMapIndex += direction;
            var curveMapView = mapViewPool.Pop(curveMapParameters.CurveMapViewReference);
            curveMapView.transform.SetParent(mapParent);
            curveMapView.transform.localScale = Vector3.one;
            
            var mapParentRectTransform = mapParent as RectTransform;
            var curveMapViewRectTransform = curveMapView.transform as RectTransform;
            var y = direction * mapParentRectTransform.rect.height;
            curveMapView.transform.localPosition = new Vector3(0, y, 0);
            curveMapViewRectTransform.sizeDelta = Vector2.zero;
            mapViewPoolSortedList[data.CurrentTotalMapIndex] = curveMapView;

            int countLevels = 0;
            for (int i = 0; i < data.CurrentTotalMapIndex; i++)
            {
                countLevels += curveMapParameters.MapDatas[i % curveMapParameters.MapDatas.Count].CellAnchors.Count;
            }
            var mapData = curveMapParameters.MapDatas[data.CurrentTotalMapIndex % curveMapParameters.MapDatas.Count];
            curveMapView.SetBackgroundMap(mapData.MapImage);
            curveMapView.SwitchMapState(data.CurrentTotalMapIndex <= data.LastTotalOpenedMapIndex);
            for (int i = 0; i < mapData.CellAnchors.Count; i++)
            {
                var cellAnchor = mapData.CellAnchors[i % mapData.CellAnchors.Count];
                SetupCellMap(curveMapView, cellAnchor, i + countLevels + 1);
            }
        }

        private void SetupCellMap(CurveMapView curveMapView, CellAnchor cellAnchor, int levelNumber)
        {
            var cellMapView = cellMapViewPool.Pop(curveMapParameters.CurveCellMapViewReference);
            cellMapView.transform.SetParent(curveMapView.CellMapParent);
            cellMapView.transform.localScale = Vector3.one;
            var rectTransform = cellMapView.transform as RectTransform;
            rectTransform.anchorMax = cellAnchor.Max;
            rectTransform.anchorMin = cellAnchor.Mix;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;
            cellMapView.Show(levelNumber, curveMapParameters);
            
            var cellLevelMapState = levelNumber == data.SelectedLevelIndex + 1
                ? ECellMapState.Current
                : levelNumber <= data.LastOpenedLevelIndex
                    ? ECellMapState.Open
                    : ECellMapState.Close;
            cellMapView.SetCellType(cellLevelMapState);
            cellMapView.EventCellLevelMapSelected += OnEventCellLevelMapSelected;
            if (cellLevelMapState != ECellMapState.Close)
            {
                var levelData = levelDatas[levelNumber - 1];
                openCellMapViewDictionary[cellMapView] = levelData;
                int numberStars = gameDataModel.GetNumberLevelProgressStars(levelNumber - 1);
                cellMapView.SetProgressStars(numberStars);
            }
            else
            {
                cellMapView.SetProgressStars(0);
            }

            allCellMapViewDictionary[cellMapView] = levelNumber;
            if (!mapViewDictionary.ContainsKey(curveMapView))
            {
                mapViewDictionary[curveMapView] = new List<CurveCellMapView>();
            }
            mapViewDictionary[curveMapView].Add(cellMapView);
        }

        private async UniTask MoveMapAsync(int direction)
        {
            if (direction == 0)
            {
                return;
            }

            var swipeAudioName = direction > 0 ? AudioNameData.SWIPE_UP_CURVE_MAP : AudioNameData.SWIPE_DOWN_CURVE_MAP;
            audioController.TryPlaySound(swipeAudioName);

            var mapParentRectTransform = mapParent as RectTransform;
            var nextMapView = mapViewPoolSortedList[data.CurrentTotalMapIndex];
            var currentMapView = mapViewPoolSortedList[data.CurrentTotalMapIndex - direction];
            var currentMapViewHeight = mapParentRectTransform.rect.height * -direction;

            var task1 = currentMapView.MoveAnimationAsync(Vector3.up * currentMapViewHeight, curveMapParameters.SwipeMapDuration);
            var task2 = nextMapView.MoveAnimationAsync(Vector3.zero, curveMapParameters.SwipeMapDuration);

            await UniTask.WhenAll(task1, task2);
        }
        
        private void HideMap(int direction)
        {
            var index = data.CurrentTotalMapIndex - direction;
            if (!mapViewPoolSortedList.ContainsKey(index))
            {
                return;
            }
            var curveMapView = mapViewPoolSortedList[index];
            var cellLevelMapViews= mapViewDictionary[curveMapView];
            foreach (var cellLevelMapView in cellLevelMapViews)
            {
                cellLevelMapView.Hide();
                cellLevelMapView.EventCellLevelMapSelected -= OnEventCellLevelMapSelected;
                openCellMapViewDictionary.Remove(cellLevelMapView);
                cellMapViewPool.Push(cellLevelMapView);
            }

            mapViewPool.Push(curveMapView);
            mapViewPoolSortedList.Remove(index);
            mapViewDictionary.Remove(curveMapView);
        }

        private bool CheckProgressStarsSelectedLevel()
        {
            var progressStarsSelectedLevel = gameDataModel.ProgressStarsSelectedLevel;
            if (progressStarsSelectedLevel.Index == -1)
            {
                return false;
            }
            
            var currentNumberStars = gameDataModel.GetNumberLevelProgressStars(progressStarsSelectedLevel.Index);
            var oldNumberStars = progressStarsSelectedLevel.Stars;
            var numberStars = currentNumberStars - oldNumberStars;
            if (numberStars == 0)
            {
                return false;
            }

            return true;
        }

        private void FlyingStarsAsync(bool isPlaySound)
        {
            var progressStarsSelectedLevel = gameDataModel.ProgressStarsSelectedLevel;
            if (progressStarsSelectedLevel.Index == -1)
            {
                EventCurveMapActive.OnNext(false);
                return;
            }
            
            var currentNumberStars = gameDataModel.GetNumberLevelProgressStars(progressStarsSelectedLevel.Index);
            var oldNumberStars = progressStarsSelectedLevel.Stars;
            var numberStars = currentNumberStars - oldNumberStars;
            if (numberStars == 0)
            {
                EventCurveMapActive.OnNext(false);
                return;
            }

            var curveCellMapView = allCellMapViewDictionary.FirstOrDefault(v => v.Value == progressStarsSelectedLevel.Index + 1).Key;
            if (curveCellMapView == null)
            {
                EventCurveMapActive.OnNext(false);
                return;
            }

            var mapIndex = GetCurrentTotalMapIndex();
            var levelCount = 0;
            var isOpenNewMap = false;
            for (int i = 0; i < mapIndex + 1; i++)
            {
                var mapData = curveMapParameters.MapDatas[i % curveMapParameters.MapDatas.Count];
                levelCount += mapData.CellAnchors.Count;
                if (levelCount == data.LastOpenedLevelIndex - 1)
                {
                    isOpenNewMap = true;
                    EventCurveMapActive.OnNext(true);
                }
            }

            if (!isOpenNewMap)
            {
                var sequence = DOTween.Sequence();
                sequence.SetDelay(0.5f).OnKill(() =>
                {
                    EventCurveMapActive.OnNext(false);
                });
            }

            SetActiveSwitchMap(false);
            
            var starsTransforms = new List<Transform>(curveCellMapView.EmptyStarsTransforms);
            for (int i = 0; i < oldNumberStars; i++)
            {
                starsTransforms.RemoveAt(0);
            }
            curveCellMapView.SetProgressStars(oldNumberStars);
            
            if(isPlaySound)
            {
                audioController.TryPlaySound(AudioNameData.ADDING_STARS_IN_MENU_SOUND);
            }
            levelMapProgressAnimator.ShowFlyingStarsAnimation(starsTransforms, numberStars, () =>
            {
                curveCellMapView.SetProgressStars(currentNumberStars);
                if (isOpenNewMap)
                {
                    OpenNewMap(isPlaySound);
                }
                else
                {
                    SetActiveSwitchMap(true);
                    EventCurveMapActive.OnNext(false);
                }
            }, () =>
            {
                if (isPlaySound)
                {
                    audioController.TryPlaySound(AudioNameData.FLIGHT_OF_STARS_TO_STARS);
                }
            });
        }

        private async void OpenNewMap(bool isPlaySound)
        {
            SetupMap(1);
            await MoveMapAsync(1);
            HideMap(1);
            if (isPlaySound)
            {
                audioController.TryPlaySound(AudioNameData.OPEN_NEW_MAP);
            }
            var mapView = mapViewPoolSortedList[data.CurrentTotalMapIndex];
            await mapView.OpenMapAnimationAsync();
            data.LastTotalOpenedMapIndex++;
            
            SetActiveSwitchMap(true);
            EventCurveMapActive.OnNext(false);
        }

        private void SetActiveSwitchMap(bool isActive)
        {
            curveMapObserver.SetActiveSwipe(isActive);
            downMapButton.interactable = isActive;
            upMapButton.interactable = isActive;
        }
    }
}