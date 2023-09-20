using UnityEngine;

namespace Game.Utils.Utils
{
    public class DontDestroyGameObject : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
