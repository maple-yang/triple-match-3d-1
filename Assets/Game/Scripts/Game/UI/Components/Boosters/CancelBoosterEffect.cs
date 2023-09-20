using Game.Data.Models;

namespace Game.UI.Components
{
    public class CancelBoosterEffect : BoosterEffect
    {
        public override void ShowEffect()
        {
            audioController.TryPlaySound(AudioNameData.BOOSTER_CANCEL);
        }

        public override void HideEffect()
        {
        }
    }
}