using System.Collections.Generic;
using Game.Core.Components;
using Game.Core.Configurations;
using Game.Core.Levels;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.Configurations
{
    [CreateAssetMenu(fileName = nameof(LevelsConfig), menuName = "Configs/GameCore/"+nameof(LevelsConfig), order = 0)]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField]
        private List<LevelData> levelsData;
        
        [SerializeField]
        private Transform parent;

        public IReadOnlyCollection<LevelData> LevelsData => levelsData;
        
        [Button]
        private void CheckValidLevelData()
        {
            foreach (Transform item in parent)
            {
                var itemView = item.GetComponent<ItemView>();
                GameObject renderObject = new GameObject();
                renderObject.transform.SetParent(itemView.transform);
                renderObject.transform.localPosition = Vector3.zero;
                renderObject.transform.eulerAngles = itemView.MeshRenderer.transform.eulerAngles;
                renderObject.name = "PointerHandler";
                var meshCollider = renderObject.AddComponent<MeshCollider>();
                MeshCollider collider = itemView.Collider as MeshCollider;
                meshCollider.sharedMesh = collider.sharedMesh;
                meshCollider.convex = true;
                var pointerHandler = renderObject.AddComponent<PointerHandler>();
                
                DestroyImmediate(itemView.Collider);
                DestroyImmediate(itemView.PointerHandler);
                itemView.PointerHandler = pointerHandler;
                itemView.Collider = meshCollider;
            }
#if UNITY_EDITOR
            bool isCorrect = true;
            string[] results = AssetDatabase.FindAssets($"ItemsPreset", new[] { "Assets/Game/Content/Configs"});
            foreach (string result in results)
            {
                var path = AssetDatabase.GUIDToAssetPath(result);
                var itemsPreset = AssetDatabase.LoadAssetAtPath<ItemsPreset>(path);
                if (itemsPreset != null)
                {
                    foreach (var data in levelsData)
                    {
                        foreach (var levelSettings in data.LevelSettings.Items)
                        {
                            var item = itemsPreset.GetItemByID(levelSettings.ItemID);
                            if (item == null)
                            {
                                Debug.LogWarning($"Level id \'{data.LevelID}\' wrong item id \'{levelSettings.ItemID}\'");
                                isCorrect = false;
                            }
                            else
                            {
                                levelSettings.Icon = itemsPreset.GetItemGoalSpriteByID(levelSettings.ItemID);
                            }
                        }
                    }
                    break;
                }
            }

            if (isCorrect)
            {
                Debug.Log($"No errors found");
            }
#endif
        }
    }
}