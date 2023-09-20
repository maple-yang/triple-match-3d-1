using Cysharp.Threading.Tasks;
using Game.Levels;

namespace Game.Tutor
{
    public interface IGameTutorManager
    {
        public UniTask TryShowTutorAsync(GameLevel gameLevel);

    }
}