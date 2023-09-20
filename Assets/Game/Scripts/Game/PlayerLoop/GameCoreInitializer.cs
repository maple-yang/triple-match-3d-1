using UnityEngine;
using Zenject;

namespace Game.PlayerLoop
{
    public class GameCoreInitializer : MonoBehaviour, IInitializable
    {
        [Inject]
        private IPlayerLoopFacade playerLoopFacade;
        
        public void Initialize()
        {
            playerLoopFacade.Initialize();
        }
    }
}
