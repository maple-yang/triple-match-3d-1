using System.Collections.Generic;
using DG.Tweening;
using Game.Core.Configurations;
using Game.Providers.Goal;
using Game.Scripts.Game.Core.Configurations;
using Game.Scripts.Game.UI.Widgets;
using UnityEngine;
using Zenject;

namespace Game.UI.Widgets
{
    public class GoalWidget : MonoBehaviour
    {
        [SerializeField]
        private ItemGoalView itemGoalViewReference;
        
        [SerializeField]
        private Transform goalWidgetParent;
        
        [Inject]
        private IGoalProvider goalProvider;
        
        [Inject] 
        private GoalParameters goalParameters;
        
        [Inject] 
        private ItemsPreset itemsPreset;
        
        private float delayBeforeHidingGoal;
        private Dictionary<string, ItemGoalView> itemGoalViewDictionary;
        private Dictionary<ItemGoalView, int> itemGoalCountDictionary;
        
        public Dictionary<string, ItemGoalView> ItemGoalViewDictionary => itemGoalViewDictionary;
        
        public void Show()
        {
            itemGoalViewDictionary = new Dictionary<string, ItemGoalView>();
            itemGoalCountDictionary = new Dictionary<ItemGoalView, int>();
            delayBeforeHidingGoal = goalParameters.DelayBeforeHidingGoal;

            var itemGoalDatas = goalProvider.GetRemainingItemGoalData();
            foreach (var itemGoalData in itemGoalDatas)
            {
                var itemGoalSprite = itemsPreset.GetItemGoalSpriteByID(itemGoalData.Key);
                var itemGoalView = Instantiate(itemGoalViewReference, goalWidgetParent);
                itemGoalView.Show(itemGoalSprite, goalParameters);
                itemGoalView.UpdateItemCountView(itemGoalData.Value);
                itemGoalViewDictionary[itemGoalData.Key] = itemGoalView;
                itemGoalCountDictionary[itemGoalView] = itemGoalData.Value;
            }
        }

        public void Hide()
        {
            foreach (var itemGoalView in itemGoalViewDictionary)
            {
                Destroy(itemGoalView.Value.gameObject);
            }
            
            itemGoalViewDictionary.Clear();
            itemGoalCountDictionary.Clear();
        }
        
        public void AddItemGoal(string itemId)
        {
            if (itemGoalViewDictionary.TryGetValue(itemId, out var itemGoalView))
            {
                if (itemGoalCountDictionary.ContainsKey(itemGoalView))
                {
                    itemGoalCountDictionary[itemGoalView]++;
                    goalProvider.UpdateItemGoalData(itemId, 1);
                    itemGoalView.UpdateItemCountView(itemGoalCountDictionary[itemGoalView]);
                }
            }
        }

        public void RemoveItemGoal(string itemId)
        {
            if (itemGoalViewDictionary.TryGetValue(itemId, out var itemGoalView))
            {
                if (itemGoalCountDictionary.ContainsKey(itemGoalView))
                {
                    itemGoalCountDictionary[itemGoalView]--;
                    itemGoalView.UpdateItemCountView(itemGoalCountDictionary[itemGoalView]);
                    
                    if (itemGoalCountDictionary[itemGoalView] <= 0)
                    {
                        itemGoalView.ItemGoalAchieved();
                        itemGoalViewDictionary.Remove(itemId);
                        itemGoalCountDictionary.Remove(itemGoalView);
                
                        Sequence sequence = DOTween.Sequence();
                        sequence.SetDelay(delayBeforeHidingGoal).OnKill(() =>
                        {
                            itemGoalView.Hide();
                        });
                    }
                    
                    goalProvider.UpdateItemGoalData(itemId, -1);
                }
            }
        }
    }
}