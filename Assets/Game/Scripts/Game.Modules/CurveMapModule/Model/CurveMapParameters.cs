using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Modules.CurveMapModule.Model
{
    [CreateAssetMenu(fileName = nameof(CurveMapParameters), menuName = "Configs/Modules/"+nameof(CurveMapParameters), order = 0)]

    public class CurveMapParameters : ScriptableObject
    {
        [field: SerializeField, TitleGroup("Animation settings")]
        public Vector3 SelectedCellScaleStart { get; private set; } = Vector3.one * 1.2f;
        
        [field: SerializeField, TitleGroup("Animation settings")]
        public Vector3 SelectedCellScaleEnd { get; private set; } = Vector3.one * 1.2f;
        
        [field: SerializeField, TitleGroup("Animation settings")]
        public float SelectedCellDuration { get; private set; } = 0.5f;
        
        [field: SerializeField, TitleGroup("Animation settings")]
        public AnimationCurve SelectedCellCurve { get; private set; }
        
        [field: SerializeField, TitleGroup("Animation settings")]
        public float SwipeMapDuration { get; private set; } = 0.5f;
        
        [field: SerializeField, TitleGroup("Components")] 
        public CurveMapView CurveMapViewReference { get; private set; }
        
        [field:  SerializeField, TitleGroup("Components")] 
        public CurveCellMapView CurveCellMapViewReference { get; private set; }
        
        [field: SerializeField, TitleGroup("Components")]
        public string PathMapImages { get; private set; } = "Assets/Game/Content/Sprites/CurveLeveMap";

        [SerializeField, TitleGroup("Map data")]
        public List<MapData> mapDatas;
        
        public List<MapData> MapDatas => mapDatas;

        [Serializable]
        public class MapData
        {
            [SerializeField]
            public string MapName;

            [SerializeField, PreviewField(Alignment = ObjectFieldAlignment.Left)]
            public Sprite MapImage;
            
            [SerializeField] 
            public List<Vector3> CellMapPositions;
            
            [SerializeField] 
            public List<CellAnchor> CellAnchors;
        }
    }
    [Serializable]
    public class CellAnchor
    {
        public Vector2 Mix;
        public Vector2 Max;
    }
}
