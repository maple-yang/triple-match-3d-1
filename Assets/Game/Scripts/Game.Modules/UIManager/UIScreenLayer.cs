using System;
using UnityEngine;

namespace Game.Modules.UIManager
{
    [Serializable]
    public class UIScreenLayer
    {
        [field: SerializeField]
        public string ID { get; private set; } = "default_layer";
        
        [field: SerializeField]
        public Canvas Canvas { get; private set; }
    }
}