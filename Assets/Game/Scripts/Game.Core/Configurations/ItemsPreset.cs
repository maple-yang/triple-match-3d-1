using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Components;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.Core.Configurations
{
    [CreateAssetMenu(fileName = nameof(ItemsPreset), menuName = "Configs/Game/" + nameof(ItemsPreset), order = 0)]
    public class ItemsPreset : ScriptableObject
    {
        [Serializable]
        public class ItemViewData
        {
            [ReadOnly] 
            public string Id;
            public ItemView ItemView;
            [PreviewField(Alignment = ObjectFieldAlignment.Left)]
            public Sprite ItemGoalSprite;
        }
        
        [SerializeField]
        private List<ItemViewData> itemViewDatas;
        
        public ItemView GetItemByID(string id)
        {
            var item = itemViewDatas.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                Debug.LogError($"No item with id \'{id}\'");
                return null;
            }
            return item.ItemView;
        }
        
        public Sprite GetItemGoalSpriteByID(string id)
        {
            var item = itemViewDatas.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                Debug.LogError($"No item with id \'{id}\'");
            }
            return item.ItemGoalSprite;
        }
        
        [OnInspectorGUI]
        private void UpdateItemId()
        {
#if UNITY_EDITOR
    
            if (itemViewDatas is { Count: > 0 })
            {
                foreach (var itemViewData in itemViewDatas)
                {
                    if(itemViewData.ItemView != null)
                    {
                        itemViewData.Id = itemViewData.ItemView.ID;

                        if (itemViewData.ItemGoalSprite == null)
                        {
                            string[] results = AssetDatabase.FindAssets($"{itemViewData.ItemView.name} t:texture2D", new[] { "Assets/Kit/items"});
                            foreach (string result in results)
                            {
                                var path = AssetDatabase.GUIDToAssetPath(result);
                                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                                if (sprite != null && sprite.name == itemViewData.ItemView.name)
                                {
                                    itemViewData.ItemGoalSprite = sprite;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            EditorUtility.SetDirty(this);
#endif
        }
    }
}