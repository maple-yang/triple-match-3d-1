using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ComponentPool<T, K> where K : Component
{
    private Dictionary<T, K> prefabs = new Dictionary<T, K>();
    private Dictionary<T, Stack<K>> components = new Dictionary<T, Stack<K>>();

    private T defaultKey = default;

    public K Pop(K defaultPrefab)
    {
        return Pop(defaultKey, defaultPrefab);
    }

    private K Pop(T key, K defaultPrefab)
    {
        K resultComponent = null;

        if (components.ContainsKey(key)
            && components[key] != null
            && components[key].Count > 0 && components[key].FirstOrDefault())
        {
            resultComponent = components[key].Pop();
        }
        else if (prefabs.ContainsKey(key))
        {
            K prefab = prefabs[key];
            K newGameObject = Object.Instantiate(prefab);
            resultComponent = newGameObject;
        }
        else
        {
            prefabs[key] = defaultPrefab;
            K newGameObject = Object.Instantiate(defaultPrefab);
            resultComponent = newGameObject;
        }

        if (resultComponent)
        {
            resultComponent.gameObject.SetActive(true);
        }

        return resultComponent;
    }

    public void Push(K go)
    {
        if (go
            && (!components.ContainsKey(defaultKey)
                || !components[defaultKey].Contains(go)))
        {
            go.gameObject.SetActive(false);

            if (components.Count == 0)
                components[defaultKey] = new Stack<K>();

            components[defaultKey].Push(go);
        }
    }
}