using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Core.Configurations
{
    [Serializable]
    public class LevelSettings
    {
        [field: SerializeField]
        public TimerParameter Timer { get; set; }

        [field: SerializeField]
        public List<ItemParameter> Items { get; set; }

        [field: SerializeField]
        public ProgressStartsParameter[] ProgressStarts { get; set; } =
        {
            new ProgressStartsParameter{NumberStar = 1, PercentTimeReceive = 10},
            new ProgressStartsParameter{NumberStar = 2, PercentTimeReceive = 30},
            new ProgressStartsParameter{NumberStar = 3, PercentTimeReceive = 60},
        };

        private Dictionary<string, ItemParameter> targetItemDictionary = new Dictionary<string, ItemParameter>();

        [OnInspectorGUI]
        private void UpdateItemParameter()
        {
#if UNITY_EDITOR
    
            targetItemDictionary.Clear();
            if (Items is { Count: > 0 })
            {
                foreach (var item in Items)
                {
                    var itemsCount = item.ItemsCount / 3;
                    item.ItemsCount = itemsCount * 3;

                    if (!string.IsNullOrEmpty(item.ItemID))
                    {
                        targetItemDictionary[item.ItemID] = item;
                    }
                }
                
                foreach (var item in Items)
                {
                    var targetItem = targetItemDictionary.FirstOrDefault(t => t.Key == item.ItemID).Value;
                    if (targetItem == null)
                    {
                        return;
                    }

                    var itemTargetCount = item.TargetCount / 3;
                    item.TargetCount = itemTargetCount * 3;
                    if (item.TargetCount > targetItem.ItemsCount)
                    {
                        item.TargetCount = targetItem.ItemsCount;
                    }
                }
            }

            if (ProgressStarts == null || ProgressStarts.Length != 3)
            {
                ProgressStarts = new []
                {
                    new ProgressStartsParameter{NumberStar = 1, PercentTimeReceive = 10},
                    new ProgressStartsParameter{NumberStar = 2, PercentTimeReceive = 30},
                    new ProgressStartsParameter{NumberStar = 3, PercentTimeReceive = 60},
                };
            }
#endif
        }
    }
}

    [Serializable]
    public class ItemParameter
    {
        [PreviewField(Alignment = ObjectFieldAlignment.Left), ReadOnly]
        public Sprite Icon;
        
        [field: SerializeField]
        public string ItemID { get; set; }
        
        [field: SerializeField]
        [field: Range(0, 51)]
        public int ItemsCount { get; set; }
        
        [field: SerializeField]
        [field: Range(0, 51)]
        public int TargetCount { get; set; }
    }

    [Serializable]
    public class TimerParameter
    {
        [SerializeField]
        [HorizontalGroupAttribute("Timer")]
        [LabelText("Min:")]
        [LabelWidth(30)]
        private float minutes = 0;
        
        [SerializeField]
        [HorizontalGroupAttribute("Timer")]
        [LabelText("Sec:")]
        [LabelWidth(30)]
        private float seconds = 0;

        public float Seconds => seconds;
        public float Minutes => minutes;
        public float TotalSeconds => seconds + (minutes * 60f);
    }
    
    [Serializable]
    public class ProgressStartsParameter
    {
        [field: SerializeField] 
        [ReadOnly]
        public int NumberStar;

        [field: SerializeField] 
        [field: Range(0, 100)]
        public int PercentTimeReceive;
    }
