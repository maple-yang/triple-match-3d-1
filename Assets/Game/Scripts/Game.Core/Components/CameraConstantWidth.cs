using UnityEngine;

namespace Game.Core.Components
{
    public class CameraConstantWidth : MonoBehaviour
    {
        [SerializeField]
        private Vector2 defaultResolution = new Vector2(1080, 1920);
        
        [Range(0f, 1f)]
        [SerializeField]
        private float widthOrHeight = 0;
        
        private Camera componentCamera;
    
        private float initialSize;
        private float targetAspect;

        private float initialFov;
        private float horizontalFov = 120f;

        public float DefaultInitialSize => initialSize;

        private void Awake()
        {
            componentCamera = GetComponent<Camera>();
            initialSize = componentCamera.orthographicSize;

            targetAspect = defaultResolution.x / defaultResolution.y;

            initialFov = componentCamera.fieldOfView;
            horizontalFov = CalcVerticalFov(initialFov, 1 / targetAspect);

            UpdateCameraOrthographic();
        }

        private void UpdateCameraOrthographic()
        {
            if (componentCamera.orthographic)
            {
                float constantWidthSize = initialSize * (targetAspect / componentCamera.aspect);
                componentCamera.orthographicSize = Mathf.Lerp(constantWidthSize, initialSize, widthOrHeight);
            }
            else
            {
                float constantWidthFov = CalcVerticalFov(horizontalFov, componentCamera.aspect);
                componentCamera.fieldOfView = Mathf.Lerp(constantWidthFov, initialFov, widthOrHeight);
            }
        }

        private float CalcVerticalFov(float hFovInDeg, float aspectRatio)
        {
            float hFovInRads = hFovInDeg * Mathf.Deg2Rad;

            float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);

            return vFovInRads * Mathf.Rad2Deg;
        }
    }
}