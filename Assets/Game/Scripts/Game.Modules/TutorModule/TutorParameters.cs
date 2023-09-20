using System.Collections;
using Game.Modules.TutorModule.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Modules.TutorModule
{
    public class TutorParameters : ScriptableObject
    {
        [System.Serializable]
        public class TutorData
        {
            public BaseTutorView TutorViewReference = null;
            [ValueDropdown("TutorLogics"), SerializeReference]
            public ITutorLogic TutorLogic;

            public static IEnumerable TutorLogics = new ValueDropdownList<ITutorLogic>();
        }
        
        [field: SerializeField] 
        public TutorData[] TutorDatas { get; private set; }
    }
}