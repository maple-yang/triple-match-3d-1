using System;
using UnityEngine;

namespace Game.Utils.Utils.Animation
{
    public class GiftAnimatorEventListener : MonoBehaviour
    {
        // Events
        public event Action OnGiftOpenFinishedEvent;


        /// <summary>
        /// 
        /// </summary>
        private void OnGiftOpenFinished()
        {
            OnGiftOpenFinishedEvent?.Invoke();
        }
    }
}