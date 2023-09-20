using Game.Data.Models;
using Game.Modules.AudioManager;
using UnityEngine;
using Zenject;

namespace Game.UI.Components
{
    public class BoosterEffect : MonoBehaviour
    {
        [SerializeField] 
        public BoosterType BoosterType;
        
        [Inject]
        protected IAudioController audioController;

        public virtual void ShowEffect()
        {
            
        }

        public virtual void HideEffect()
        {
            
        }
    }
}