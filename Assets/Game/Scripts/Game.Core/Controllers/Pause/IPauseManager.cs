namespace Game.Core.Controllers
{
    public interface IPauseManager
    {
        public void AddPauseComponent(IPauseController pauseController);
    }
}