using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Levels;
using UnityEngine;
using Zenject;

namespace Game.Modules.UIManager
{
    public sealed class UIScreenManager : MonoBehaviour, IUIScreenManager, IInitializable
    {
        [SerializeField]
        private UIScreenLayer[] contentLayers;
        
        [Inject]
        private readonly UIPanels panels;

        [Inject]
        private readonly DiContainer Container;

        private List<UIPanel<UIContext>> activePanels;
        
        private Dictionary<string, IPanelCommand> loadPanelCommands;

        public void Initialize()
        {
            activePanels = new List<UIPanel<UIContext>>();
            loadPanelCommands = new Dictionary<string, IPanelCommand>();
        }

        public bool IsPanelShowed<TPanel>() where TPanel : UIPanel<UIContext>
        {
            var panel = activePanels.SingleOrDefault(t => t.GetType() == typeof(TPanel));
            return panel != null && panel.IsActive;
        }

        public async void ShowUIPanel<TPanel>(UIContext context = null, bool instant = false, Action<TPanel> panelShowed = null) where TPanel : UIPanel<UIContext>
        {
            if (activePanels.Exists(t => t.GetType() == typeof(TPanel)))
            {
                Debug.LogWarning($"Panel with type \'{typeof(TPanel)}\' already showed!");
                return;
            }

            var panelGuid = panels.TryGetPanelGuid<TPanel>();
            if (panelGuid.Item1 != null)
            {
                var panelCommand = new LoadPanelCommand(panelGuid.Item1);
                loadPanelCommands.Add(panelGuid.Item1, panelCommand);

                //var result = await panelCommand.Execute<TPanel>();
                var result = panelGuid.Item2.GetComponent<TPanel>();
                ShowPanelAfterLoad(result, context, instant, panelShowed);

                // task.GetAwaiter().OnCompleted(() =>
                // {
                //     var result = task.GetAwaiter().GetResult();
                //     ShowPanelAfterLoad(result, context, instant, panelShowed);
                // });
            }
        }

        public async UniTask<TPanel> ShowUIPanelAsync<TPanel>(UIContext context = null, bool instant = false) where TPanel : UIPanel<UIContext>
        {
            if (activePanels.Exists(t => t.GetType() == typeof(TPanel)))
            {
                Debug.LogWarning($"Panel with type \'{typeof(TPanel)}\' already showed!");
                return null;
            }
            
            var panelGuid = panels.TryGetPanelGuid<TPanel>();
            if (panelGuid.Item1 != null)
            {
                var panelCommand = new LoadPanelCommand(panelGuid.Item1);
                loadPanelCommands.Add(panelGuid.Item1, panelCommand);

                // var result = await panelCommand.Execute<TPanel>();
                var result = panelGuid.Item2.GetComponent<TPanel>();
                var panel = ShowPanelAfterLoad(result, context, instant);
                await UniTask.WaitWhile(() => panel.IsActive == false);
                return panel;
            }

            return null;
        }

        public void UpdateUIPanel<TPanel>(UIContext context)
        {
            var activePanel = activePanels.SingleOrDefault(t => t.GetType() == typeof(TPanel));
            
            if (activePanel == null)
            {
                Debug.LogWarning($"Panel with type \'{typeof(TPanel)}\' isn't showed!");
                return;
            }
            
            activePanel.UpdatePanel(context);
        }

        public void HideUIPanel<TPanel>(bool instant = false, Action panelHidden = null) where TPanel : UIPanel<UIContext>
        {
            var activePanel = activePanels.SingleOrDefault(t => t.GetType() == typeof(TPanel));
            
            if (activePanel != null)
            {
                activePanel.Hide(instant, () =>
                {
                    DisposePanel((TPanel)activePanel);
                    activePanels.Remove(activePanel);
                    panelHidden?.Invoke();
                });
                return;
            }

            DisposePanel((TPanel)activePanel);
            panelHidden?.Invoke();
        }

        public async UniTask HideUIPanelAsync<TPanel>(bool instant = false) where TPanel : UIPanel<UIContext>
        {
            var activePanel = activePanels.SingleOrDefault(t => t.GetType() == typeof(TPanel));
            if (activePanel != null)
            {
                activePanel.Hide(instant);
                
                await UniTask.WaitWhile(() => activePanel.IsActive);
                activePanels.Remove(activePanel);
            }

            DisposePanel((TPanel)activePanel);
        }

        public bool TryGetUIPanel<TPanel>(out TPanel panel) where TPanel : UIPanel<UIContext>
        {
            var result = activePanels.SingleOrDefault(t => t.GetType() == typeof(TPanel));
            panel = result as TPanel;
            return panel != null && panel;
        }

        private TPanel ShowPanelAfterLoad<TPanel>
            (TPanel uiPanel, UIContext context, bool showInstant = false, Action<TPanel> panelShowed = null)
            where TPanel : UIPanel<UIContext>
        {
            var layer = GetContentLayer(uiPanel.LayerID);
            var instantiate = Container.InstantiatePrefab(uiPanel, layer.Canvas.transform);
            var panel = instantiate.GetComponent<TPanel>();
            if (layer.Canvas.transform.childCount != 0)
            {
                layer.Canvas.gameObject.SetActive(true);
            }
            
            panel.Initialize(context);
            panel.Show(showInstant, () =>
            {
                panelShowed?.Invoke(panel);
            });
            activePanels.Add(panel);
            return panel;
        }

        private void DisposePanel<TPanel>(TPanel panelInstance = null) where TPanel : UIPanel<UIContext>
        {
            var panelCommand = GetPanelCommand<TPanel>();
            var key = loadPanelCommands.FirstOrDefault(t => t.Value == panelCommand).Key;
            panelCommand?.Dispose();
            if(key != null && loadPanelCommands.ContainsKey(key))
            {
                loadPanelCommands.Remove(key);
            }

            if (panelInstance != null)
            {
                var layer = GetContentLayer(panelInstance.LayerID);
                panelInstance.DeInitialize();
                Destroy(panelInstance.gameObject);
                if (layer.Canvas.transform.childCount == 0)
                {
                    layer.Canvas.gameObject.SetActive(false);
                }
            }
        }
        
        private IPanelCommand GetPanelCommand<TPanel>() where TPanel : UIPanel<UIContext>
        {
            var panelGuid = panels.TryGetPanelGuid<TPanel>();
            if (panelGuid.Item1 != null && loadPanelCommands.ContainsKey(panelGuid.Item1))
            {
                return loadPanelCommands[panelGuid.Item1];
            }
            
            Debug.LogWarning($"Panel with type \'{typeof(TPanel)}\' already hidden!");
            return null;
        }

        private UIScreenLayer GetContentLayer(string layerId)
        {
            var layer = contentLayers.SingleOrDefault(t => t.ID == layerId);
            if (layer == null)
            {
                Debug.LogError($"There's no layer with id \'{layerId}\'!");
                return null;
            }

            return layer;
        }
    }
}
