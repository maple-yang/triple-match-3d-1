using System.Collections.Generic;
using System.Linq;
using Game.Core.Components;
using Game.Core.Configurations;
using UnityEngine;

namespace Game.Levels
{
    public class LevelCreator : ILevelCreator
    {
        private readonly ItemView.Factory itemFactory;

        private readonly List<float> randomPositionsX = new ();
        private readonly List<float> randomPositionsY = new ();
        private readonly List<float> randomPositionsZ = new ();

        public LevelCreator(ItemView.Factory itemFactory)
        {
            this.itemFactory = itemFactory;
        }
        
        public List<ItemView> Create(Bounds spawnBounds, List<ItemParameter> items)
        {
            var views = new List<ItemView>();
            for (int i = 0; i < items.Count; i++)
            {
                var itemParameter = items[i];
                for (int index = 0; index < itemParameter.ItemsCount; index++)
                {
                    var instance = itemFactory.Create(itemParameter.ItemID);
                    var position = GetRandomPosition(spawnBounds);
                    var rotation = GetRandomRotation(0, 360f);
                    
                    instance.Position = position;
                    instance.Rotation = rotation;
                    
                    views.Add(instance);
                }
            }
            return views;
        }

        private Vector3 GetRandomPosition(Bounds bounds)
        {
            var randomX = GetRandom(bounds.min.x, bounds.max.x, randomPositionsX);
            var randomY = GetRandom(bounds.min.y, bounds.max.y, randomPositionsY);
            var randomZ = GetRandom(bounds.min.z, bounds.max.z, randomPositionsZ);
            
            randomPositionsX.Add(randomX);
            randomPositionsY.Add(randomY);
            randomPositionsZ.Add(randomZ);
            
            var random = new Vector3(randomX, randomY, randomZ);
            return random;
        }

        private Quaternion GetRandomRotation(float min, float max)
        {
            var angleX = Random.Range(min, max);
            var angleY = Random.Range(min, max);
            var angleZ = Random.Range(min, max);
            var random = new Vector3(angleX, angleY, angleZ);
            var randomRotation = Quaternion.Euler(random);
            return randomRotation;
        }

        private float GetRandom(float min, float max, IList<float> randomValues)
        {
            if (randomValues.Count > 0)
            {
                var halfDistance = Mathf.Abs((max - min) * 0.5f);
                var averageRandom = randomValues.Average();
                var randomMin = averageRandom < min + halfDistance ? min + halfDistance : min;
                var randomMax = randomMin + halfDistance;
                return Random.Range(randomMin, randomMax);   
            }
            
            return Random.Range(min, max);
        }
    }
}