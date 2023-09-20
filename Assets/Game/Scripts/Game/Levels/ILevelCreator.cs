using System.Collections.Generic;
using Game.Core.Components;
using Game.Core.Configurations;
using UnityEngine;

namespace Game.Levels
{
    public interface ILevelCreator
    {
        List<ItemView> Create(Bounds spawnBounds, List<ItemParameter> items);
    }
}