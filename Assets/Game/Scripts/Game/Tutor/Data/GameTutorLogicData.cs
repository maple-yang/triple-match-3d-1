using Game.Data.Models;
using Game.Levels;
using Game.Managers;
using Game.Modules.TutorModule.Data;

namespace Game.Tutor.Data
{
    public class GameTutorLogicData : TutorLogicData
    {
        public GameDataModel GameDataModel;
        public GameLevel GameLevel;
        public CameraManager CameraManager;
    }
}