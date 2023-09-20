using Game.Data.Models;

namespace Game.UI.Components
{
    public class ShuffleBoosterEffect: BoosterEffect
    {
        public override void ShowEffect()
        {
            audioController.TryPlaySound(AudioNameData.BOOSTER_SHUFFLE);
        }

        public override void HideEffect()
        {
        }
    }
}