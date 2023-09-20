using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;

namespace Game.Modules.CurveMapModule.Model
{
    public class CurveMapCellCreator : MonoBehaviour
    {
        [field: SerializeField]
        public TextMeshProUGUI CellNumberText { get; set; }

        public ISubject<Unit> EventCellChangeActiveStatus = new Subject<Unit>();

        private bool currentStatus;

        [OnInspectorGUI]
        private void OnCellChangeActiveStatus()
        {
            if (gameObject.activeSelf != currentStatus)
            {
                currentStatus = gameObject.activeSelf;
                EventCellChangeActiveStatus.OnNext(Unit.Default);
            }
        }
    }
}
