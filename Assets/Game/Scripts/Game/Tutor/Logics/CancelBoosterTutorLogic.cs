using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Modules.TutorModule.Data;
using Game.Modules.TutorModule.View;
using Game.Tutor.Data;

namespace Game.Tutor.Logics
{
    public class CancelBoosterTutorLogic : BoosterTutorLogic
    {
        public override async UniTask StartLogic(BaseTutorView tutorView, TutorLogicData tutorLogicData)
        {
            await UniTask.Delay(200);

            var data = (GameTutorLogicData)tutorLogicData;
            var remainingItems = data.GameDataModel.RemainingItemGoalData;
            var item = data.GameLevel.ItemsContainer.Items.FirstOrDefault(i => !remainingItems.Keys.Contains(i.ID));
            data.GameLevel.ItemsContainer.RemoveItem(item);
            await data.GameLevel.ItemsInventory.AddItemToFreeSlot(item);
            await base.StartLogic(tutorView, tutorLogicData);
        }
    }
}