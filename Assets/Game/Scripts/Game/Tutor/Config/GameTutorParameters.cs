using Game.Modules.TutorModule;
using Game.Tutor.Logics;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.Tutor.Config
{
    [CreateAssetMenu(fileName = nameof(GameTutorParameters), menuName = "Configs/Modules/" + nameof(GameTutorParameters), order = 0)]
    public class GameTutorParameters : TutorParameters
    {
        [OnInspectorGUI]
        private void UpdateTutorLogics()
        {
#if UNITY_EDITOR

            TutorData.TutorLogics = new ValueDropdownList<ITutorLogic>()
            {
                { "BoosterTutorLogic", new BoosterTutorLogic() },
                { "CancelBoosterTutorLogic", new CancelBoosterTutorLogic() },
                { "FirstLevelTutorLogic", new FirstLevelTutorLogic() },
            };

            EditorUtility.SetDirty(this);
#endif
        }
    }
}