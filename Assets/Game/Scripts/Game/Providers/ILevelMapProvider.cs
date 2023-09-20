using Game.Core.Levels;
using UniRx;

namespace Game.Providers
{
    public interface ILevelMapProvider
    {
        ISubject<bool> EventCurveMapActive { get; set; }
        void Initialization();
        void Show(bool isPlaySound = true);
        void Hide();
        LevelData GetSelectedLevelData();
    }
}