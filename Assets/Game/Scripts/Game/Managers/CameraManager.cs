using Game.Core.Components;
using UnityEngine;

namespace Game.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [field: SerializeField]
        public Camera MainCamera { get; set; }
        
        [field: SerializeField]
        public CameraConstantWidth CameraConstantWidth  { get; set; }
    }
}
