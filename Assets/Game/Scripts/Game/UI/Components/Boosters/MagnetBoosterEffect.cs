using Game.Data.Models;

namespace Game.UI.Components
{
    public class MagnetBoosterEffect : BoosterEffect
    {
        public override void ShowEffect()
        {
            audioController.TryPlaySound(AudioNameData.BOOSTER_MAGNET);
        }

        public override void HideEffect()
        {
        }
    }
}