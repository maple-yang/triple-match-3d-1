using System.Collections.Generic;
using UnityEngine;

namespace Game.Modules.TutorModule.View
{
    /// <summary>
    /// Unmask Raycast Filter.
    /// The ray passes through the unmasked rectangle.
    /// </summary>
    [AddComponentMenu("UI/Unmask/UnmaskRaycastFilter", 2)]
    public class UnmaskRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
    {
        //################################
        // Serialize Members.
        //################################
        [Tooltip("Target unmask component. The ray passes through the unmasked rectangle.")] [SerializeField]
        private List<Unmask> m_targetUnmasks;


        //################################
        // Public Members.
        //################################
        /// <summary>
        /// Target unmask component. Ray through the unmasked rectangle.
        /// </summary>
        public List<Unmask> TargetUnmasks
        {
            get { return m_targetUnmasks; }
            set { m_targetUnmasks = value; }
        }

        /// <summary>
        /// Given a point and a camera is the raycast valid.
        /// </summary>
        /// <returns>Valid.</returns>
        /// <param name="sp">Screen position.</param>
        /// <param name="eventCamera">Raycast camera.</param>
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (!isActiveAndEnabled)
            {
                return true;
            }

            for (int i = 0, l = TargetUnmasks.Count; i < l; i++)
            {
                Unmask unmask = TargetUnmasks[i];

                // Skip if deactived.
                if (!unmask || !unmask.isActiveAndEnabled)
                {
                    continue;
                }

                // check inside
                if (eventCamera)
                {
                    bool isRaycastLocationValid =
                        RectTransformUtility.RectangleContainsScreenPoint((unmask.transform as RectTransform), sp,
                            eventCamera);
                    if (isRaycastLocationValid)
                    {
                        return false;
                    }
                }
                else
                {
                    bool isRaycastLocationValid =
                        RectTransformUtility.RectangleContainsScreenPoint((unmask.transform as RectTransform), sp);
                    if (isRaycastLocationValid)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        //################################
        // Private Members.
        //################################

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
        }
    }
}