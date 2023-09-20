using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Game.Modules.CurveMapModule.Model
{
    public class CurveMapCreator : MonoBehaviour
    {
        [SerializeField, TitleGroup("Components")]
        private Image mapImage;
        
        [SerializeField, TitleGroup("Components")]
        private Transform cellMapParent;
        
        [SerializeField, TitleGroup("Components")]
        private CurveMapCellCreator curveMapCellCreatorRef;
        
        [ValueDropdown("SearchMapImage"), SerializeReference, TitleGroup("Creator")]
        private Sprite MapImageId;

        private CurveMapParameters curveMapParameters;
        private List<CurveMapCellCreator> curveMapCells;
        private CompositeDisposable compositeDisposable;

#if UNITY_EDITOR
        private IEnumerable SearchMapImage()
        {
            if (curveMapParameters == null)
            {
                string[] curveMapParametersResults = AssetDatabase.FindAssets($"CurveMapParameters", new[] {"Assets/Game/Content/Configs"});
                foreach (string result in curveMapParametersResults)
                {
                    var path = AssetDatabase.GUIDToAssetPath(result);
                    curveMapParameters = AssetDatabase.LoadAssetAtPath<CurveMapParameters>(path);
                    if (curveMapParameters != null)
                    {
                        break;
                    }
                }
            }

            if (!HaveCurveMapParameters())
            {
                return null;
            }

            var mapImages =  AssetDatabase.FindAssets($"t:texture2D", new[] {curveMapParameters.PathMapImages})
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<Sprite>(x));
            return mapImages;
        }
#endif

        [Button(ButtonHeight = 50)]
        private void LoadMap()
        {
            if (!HaveCurveMapParameters())
            {
                return;
            }

            if (compositeDisposable != null)
            {
                compositeDisposable.Dispose();
                compositeDisposable.Clear();
            }
            compositeDisposable = new CompositeDisposable();
            curveMapCells = new List<CurveMapCellCreator>();
            foreach (Transform child in cellMapParent)
            {
                var curveMapCell = child.GetComponent<CurveMapCellCreator>();
                curveMapCell.EventCellChangeActiveStatus.Subscribe(OnCellChangeActiveStatus).AddTo(compositeDisposable);
                curveMapCell.gameObject.SetActive(false);
                curveMapCells.Add(curveMapCell);
            }

            var mapData = curveMapParameters.MapDatas.Find(i => i.MapImage.name == MapImageId.name);
            if (mapData == null)
            {
                Debug.Log("Map is not found!");

                for (int i = 0; i < 10; i++)
                {
                    var curveMapCell = curveMapCells.Count > i ? curveMapCells[i] : CreateCurveMapCell();
                    curveMapCell.transform.position = Vector3.up * 200 * i;
                    curveMapCell.transform.SetAsLastSibling();
                    curveMapCell.CellNumberText.text = $"{i + 1}";
                    curveMapCell.gameObject.name = $"CurveMapCell:{i + 1}";
                    curveMapCell.gameObject.SetActive(true);
                }

                mapImage.sprite = MapImageId;
                return;
            }

            mapImage.sprite = mapData.MapImage;
            for (int i = 0; i < mapData.CellMapPositions.Count; i++)
            {
                //var position = mapData.CellMapPositions[i];
                var cellAnchor = mapData.CellAnchors[i];
                var curveMapCell = curveMapCells.Count > i ? curveMapCells[i] : CreateCurveMapCell();
                //curveMapCell.transform.localPosition = position;
                var rectTransform = curveMapCell.transform as RectTransform;
                rectTransform.anchorMax = cellAnchor.Max;
                rectTransform.anchorMin = cellAnchor.Mix;
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
                curveMapCell.transform.SetAsLastSibling();
                curveMapCell.CellNumberText.text = $"{i + 1}";
                curveMapCell.gameObject.name = $"CurveMapCell:{i + 1}";
                curveMapCell.gameObject.SetActive(true);
            }
            
            Debug.Log("Load successful");
        }

        private CurveMapCellCreator CreateCurveMapCell()
        {
            var curveMapCell = Instantiate(curveMapCellCreatorRef, cellMapParent);
            curveMapCell.EventCellChangeActiveStatus.Subscribe(OnCellChangeActiveStatus).AddTo(compositeDisposable);
            curveMapCells.Add(curveMapCell);
            return curveMapCell;
        }

        [Button(ButtonHeight = 50)]
        private void SaveMap()
        {
            if (!HaveCurveMapParameters())
            {
                return;
            }

            if (MapImageId.name != mapImage.sprite.name)
            {
                Debug.LogWarning("Saving is not possible, you need to load the map first");
                return;
            }

            var cellMapPositions = cellMapParent.GetComponentsInChildren<CurveMapCellCreator>()
                .Where(tr => tr.gameObject.activeInHierarchy)
                .Select(tr => tr.transform.localPosition)
                .ToList();

            var cellAnchors = new List<CellAnchor>();
            cellMapParent.GetComponentsInChildren<CurveMapCellCreator>()
                .Where(tr => tr.gameObject.activeInHierarchy)
                .Select(tr => tr.transform as RectTransform)
                .ForEach(tr =>
                {
                    var cellAnchor = new CellAnchor() {Mix = tr.anchorMin, Max = tr.anchorMax};
                    cellAnchors.Add(cellAnchor);
                });

            var newMapData = new CurveMapParameters.MapData
            {
                MapImage = MapImageId,
                MapName = MapImageId.name,
                CellMapPositions = cellMapPositions,
                CellAnchors = cellAnchors
            };

            var mapData = curveMapParameters.MapDatas.Find(i => i.MapImage.name == MapImageId.name);
            if (mapData == null)
            {
                curveMapParameters.MapDatas.Add(newMapData);
                Debug.Log("Save new map successful");
            }
            else
            {
                var index = curveMapParameters.MapDatas.IndexOf(mapData);
                curveMapParameters.MapDatas[index] = newMapData;
                Debug.Log("Override and save map successful");
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(curveMapParameters);
#endif
        }

        private void OnCellChangeActiveStatus(Unit unit)
        {
            var curveMapCells = cellMapParent.GetComponentsInChildren<CurveMapCellCreator>()
                .Where(tr => tr.gameObject.activeInHierarchy)
                .ToList();
            for (int i = 0; i < curveMapCells.Count; i++)
            {
                curveMapCells[i].CellNumberText.text = $"{i + 1}";
                curveMapCells[i].gameObject.name = $"CurveMapCell:{i + 1}";
            }
        }

        private bool HaveCurveMapParameters()
        {
            if (curveMapParameters == null)
            {
                Debug.LogError("CurveMapParameters is not found!");
                return false;
            }

            return true;
        }
    }
}