using System.Collections.Generic;

namespace Game.Providers.Goal
{
    public interface IGoalProvider
    {
        public Dictionary<string, int> GetRemainingItemGoalData();
        public void UpdateItemGoalData(string itemId, int itemCount);
    }
}