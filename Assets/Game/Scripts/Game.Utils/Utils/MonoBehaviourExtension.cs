using UnityEngine;

namespace Game.Utils.Utils
{
    public static class MonoBehaviourExtension
    {
        /// <summary>
        /// If coroutine not null - call StopCoroutine and Nullifies it
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public static void TryStopCoroutine(this MonoBehaviour _this, ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                _this.StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}