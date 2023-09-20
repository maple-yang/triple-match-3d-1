using System.Collections.Generic;

namespace Game.Core.Controllers
{
    public class PauseController : IPauseController, IPauseManager
    {
        private List<IPauseController> pauseGames = new List<IPauseController>();

        public void PauseGameOn()
        {
            foreach (var pauseGame in pauseGames)
            {
                pauseGame.PauseGameOn();
            }
        }

        public void PauseGameOff()
        {
            foreach (var pauseGame in pauseGames)
            {
                pauseGame.PauseGameOff();
            }
        }

        public void AddPauseComponent(IPauseController pauseController)
        {
            if (!pauseGames.Contains(pauseController))
            {
                pauseGames.Add(pauseController);
            }
        }
    }
}