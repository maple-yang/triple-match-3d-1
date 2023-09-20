using System;
using UniRx;

namespace Game.Data.Models
{
    [Serializable]
    public class BoostersData
    {
        public BoosterType BoosterType { get; set; }
        public IntReactiveProperty Counter { get; set; }
        public IntReactiveProperty OpenOnLevel { get; set; }
    }

    public enum BoosterType
    {
        Magnet = 0,
        Cancel = 1,
        Shuffle = 2,
        Freezing = 3
    }
}