using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
using Unity.VisualScripting;
#endif

namespace Game.Modules.UIManager
{
    [CreateAssetMenu(fileName = nameof(UIPanels), menuName = "Configs/UI/"+nameof(UIPanels), order = 0)]
    public class UIPanels : ScriptableObject
    {
        [SerializeField, OnValueChanged(nameof(OnUpdatePanelsInfo)), DrawWithUnity]
        private List<AssetReference> panelReferences;
        
        [field: SerializeField]
        public List<GameObject> panelGameObjects;

        [SerializeField, ReadOnly]
        private List<PanelsInfo> panelsInfo;

        public (string, GameObject) TryGetPanelGuid<TPanel>() where TPanel : UIPanel<UIContext>
        {
            var panelInfo = panelsInfo.SingleOrDefault(t => t.PanelName == typeof(TPanel).FullName);
            
            if (panelInfo == null)
            {
                Debug.LogError($"No panel reference in config with type \'{typeof(TPanel)}\'");
                return (null, null);
            }

            var index = panelsInfo.IndexOf(panelInfo);
            var assetReference = panelGameObjects[index];
            
            return (panelInfo.PanelGuid, assetReference);
        }

        [OnInspectorInit]
        private void OnUpdatePanelsInfo()
        {
#if UNITY_EDITOR
            panelsInfo.Clear();
            foreach (var panelReference in panelReferences)
            {
                if(panelReference == null) continue;
                
                var panelInfo = new PanelsInfo
                {
                    PanelGuid = panelReference.AssetGUID,
                    PanelName = panelReference.editorAsset.GameObject().GetComponent<UIPanel<UIContext>>().GetType().FullName
                };
                panelsInfo.Add(panelInfo);
            }
            
            EditorUtility.SetDirty(this);
#endif
        }

        [Serializable]
        public class PanelsInfo
        {
            [field: SerializeField]
            public string PanelGuid;
            
            [field: SerializeField]
            public string PanelName;
        }
    }
}