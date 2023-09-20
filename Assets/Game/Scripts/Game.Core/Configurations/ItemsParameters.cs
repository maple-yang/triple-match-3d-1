using DG.Tweening;
using UnityEngine;

namespace Game.Core.Configurations
{
    [CreateAssetMenu(fileName = nameof(ItemsParameters), menuName = "Configs/GameCore/"+nameof(ItemsParameters), order = 0)]
    public class ItemsParameters : ScriptableObject
    {
        [field: Header("Animations")]
        [field: Tooltip("Размер объекта в инвенторе")]
        [field: SerializeField]
        public float ItemsInSlotSize { get; private set; } = 0.5f;
        
        [field: SerializeField]
        [field: Tooltip("Размер объекта при мерже")]
        public float MergeScale { get; private set; } = 1f;
        
        [field: SerializeField]
        [field: Tooltip("Длительность анимации скейла объекта при мерже")]
        public float MergeScaleDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        [field: Tooltip("Изинг анимации скейла объекта при мерже")]
        public Ease MergeScaleEase { get; private set; } = Ease.Linear;

        [field: Space]
        [field: SerializeField]
        [field: Tooltip("Эффект мержа в момент коллапса")]
        public ParticleSystem MergeParticleSystem { get; private set; }
        
        [field: SerializeField]
        [field: Tooltip("Длительность перемещения объекта с поля в инветнарь")]
        public float MoveToInventoryDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        [field: Tooltip("Изинг анимации перемещения объекта с поля в инветнарь")]
        public Ease MoveToInventoryEase { get; private set; } = Ease.Linear;

        [field: Space]
        [field: SerializeField]
        [field: Tooltip("Длительность анимации мержа объектов в инвенторе")]
        public float MergeDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        [field: Tooltip("Изинг анимации мержа объектов в инвенторе")]
        public Ease MergeEase { get; private set; } = Ease.Linear;
        
        [field: Space]
        [field: SerializeField]
        [field: Tooltip("Длительность анимации перемещения объектов в инвенторе при добавлении нового")]
        public float MoveInInventoryDuration { get; private set; } = 0.5f;
        
        [field: SerializeField]
        [field: Tooltip("Изинг анимации перемещения объектов в инвенторе при добавлении нового")]
        public Ease MoveInInventoryEase { get; private set; } = Ease.Linear;
        
        [field: Space]
        [field: SerializeField] 
        [field: Tooltip("Амплитуда айдл анимации объекта в инвенторе")]
        public float IdleAnimationAmplitude = 0.15f;
        
        [field: SerializeField]
        [field: Tooltip("Длительность айдл анимации объекта в инвенторе")]
        public float IdleAnimationDuration = 1f;

        [field: SerializeField]
        [field: Tooltip("Изинг айдл анимации объекта в инвенторе")]
        public Ease IdleAnimationEase = Ease.Linear;
        
        [field: Header("Outline")]
        [field: SerializeField]
        [field: Tooltip("Размер выделения объекта при нажатии на него")]
        public float OutlineSize { get; private set; }
        
        [field: SerializeField]
        [field: Tooltip("Цвет выделения объекта при нажатии на него")]
        public Color OutlineColor { get; private set; }
    }
}