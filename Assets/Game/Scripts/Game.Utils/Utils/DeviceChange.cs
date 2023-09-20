using System;
using System.Collections;
using UnityEngine;

namespace Game.Utils.Utils
{
    public class DeviceChange : MonoBehaviour
    {
        public static float CheckDelay = 0.5f;        // How long to wait until we check again.

        private static Vector2 resolution;                    // Current Resolution
        private static DeviceOrientation orientation;        // Current Device Orientation
        private static bool isAlive = true;                    // Keep this script running?

        // Events
        public static event Action<Vector2> OnResolutionChange;
        public static event Action<DeviceOrientation> OnOrientationChange;

        private void Start()
        {
            StartCoroutine(CheckForChange());
        }

        private IEnumerator CheckForChange()
        {
            resolution = new Vector2(Screen.width, Screen.height);
            orientation = Input.deviceOrientation;
            WaitForSeconds waitForSeconds = new WaitForSeconds(CheckDelay);

            while (isAlive)
            {
                // Check for a Resolution Change
                if (Mathf.CeilToInt(resolution.x) != Screen.width
                    || Mathf.CeilToInt(resolution.y) != Screen.height)
                {
                    resolution = new Vector2(Screen.width, Screen.height);
                    OnResolutionChange?.Invoke(resolution);
                }

                // Check for an Orientation Change
                switch (Input.deviceOrientation)
                {
                    case DeviceOrientation.Unknown:            // Ignore
                    case DeviceOrientation.FaceUp:            // Ignore
                    case DeviceOrientation.FaceDown:        // Ignore
                        break;
                    default:
                        if (orientation != Input.deviceOrientation)
                        {
                            orientation = Input.deviceOrientation;
                            OnOrientationChange?.Invoke(orientation);
                        }
                        break;
                }

                yield return waitForSeconds;
            }
        }

        private void OnDestroy()
        {
            isAlive = false;
        }
    }
}