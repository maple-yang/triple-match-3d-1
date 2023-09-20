using UnityEngine;

namespace Game.Utils.Utils
{
    public class RootSingletonManager<T> : MonoBehaviour
        where T : RootSingletonManager<T>
    {
        protected static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            name = typeof(T).Name;
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            Instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
    }
}