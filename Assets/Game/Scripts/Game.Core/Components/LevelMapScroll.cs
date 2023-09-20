using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Core.Controllers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Core.Components
{
    public class LevelMapScroll : MonoBehaviour
    {
        [SerializeField] 
        private GridLayoutGroup gridLayoutGroup;
        
        [SerializeField] 
        private ScrollRect scrollRect;

        [Inject] 
        private DiContainer diContainer;

        private int dataCount;
        private RectsComparer rectsComparer;
        private SortedList<int, Component> sortedList;
        private ComponentPool<byte, Component> componentPool = new ComponentPool<byte, Component>();
        private Component prefab;
        private RectTransform prefabRectTransform;
        private List<Rect> rects;
        private Sequence sequence;

        public event Action<Component, int> EventSetupItem;
        public event Action<Component, int> EventHideItem;
        public bool IsScrollActive => scrollRect.enabled;

        public void Initialization(Component prefab, int dataCount, int startIndex)
        {
            this.prefab = prefab;
            this.dataCount = dataCount;

            prefabRectTransform = prefab.transform as RectTransform;
            rectsComparer = new RectsComparer();
            sortedList = new SortedList<int, Component>();

            CollectRects();
            RecalculateContentSize(startIndex);
            
            rectsComparer.EventRectShow += OnRectShow;
            rectsComparer.EventRectHide += OnRectHide;
            rectsComparer.SetScrollRects(scrollRect, rects);
        }
        
        public void SetActiveScrollRect(bool isActive)
        {
            scrollRect.enabled = isActive;
        }

        public void MoveScrollToIndex(int targetIndex)
        {
            UpdateScrollContentPosition(targetIndex);
        }
        
        public void MoveScrollFromIndexToIndex(int fromIndex, int toIndex)
        {
            float targetIndex = fromIndex;
            sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(()=> targetIndex, x=> targetIndex = x, toIndex, 1f)
                .OnUpdate(() =>
                {
                    UpdateScrollContentPosition(targetIndex);
                }));
        }

        private void CollectRects()
        {
            rects = new List<Rect>(dataCount);

            for (int i = 0, l = dataCount; i < l; i++)
            {
                Rect rect = new Rect(
                    x: gridLayoutGroup.padding.left + i % gridLayoutGroup.constraintCount * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x),
                    y: -(Mathf.FloorToInt(i / gridLayoutGroup.constraintCount) + 1) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y),
                    width: gridLayoutGroup.cellSize.x,
                    height:gridLayoutGroup.cellSize.y
                );
                rects.Add(rect);
            }
        }

        private void RecalculateContentSize(int startIndex)
        {
            RectTransform contentRectTransform = gridLayoutGroup.transform as RectTransform;

            if(rects == null || rects.Count == 0)
            {
                contentRectTransform.sizeDelta = Vector3.zero;
                return;
            }

            Rect contentRect = new Rect();
            contentRect.xMin = rects.Min(br => br.xMin);
            contentRect.yMin = rects.Min(br => br.yMin);
            contentRect.xMax = rects.Max(br => br.xMax);
            contentRect.yMax = rects.Max(br => br.yMax);
            contentRect.min = scrollRect.viewport.InverseTransformPoint(scrollRect.content.TransformPoint(contentRect.min));
            contentRect.max = scrollRect.viewport.InverseTransformPoint(scrollRect.content.TransformPoint(contentRect.max));

            contentRectTransform.sizeDelta = contentRect.size;
            UpdateScrollContentPosition(startIndex);
        }

        private void OnRectShow(int rectIndex, Rect rect)
        {
            SetupItem(rectIndex, rect);
        }

        private void OnRectHide(int rectIndex, Rect rect)
        {
            HideItem(rectIndex);
        }

        private void SetupItem(int rectIndex, Rect rect)
        {
            var component = componentPool.Pop(prefab);
            sortedList[rectIndex] = component;

            if (!component.gameObject.activeSelf)
            {
                component.gameObject.SetActive(true);
            }
            else
            {
                diContainer.InjectGameObject(component.gameObject);
            }

            component.transform.SetParent(scrollRect.content);
            component.transform.localScale = Vector3.one;
            component.transform.localPosition = rect.center;
            EventSetupItem?.Invoke(component, rectIndex);
        }

        private void HideItem(int rectIndex)
        {
            if (!sortedList.ContainsKey(rectIndex))
            {
                return;
            }

            var component = sortedList[rectIndex];
            sortedList.Remove(rectIndex);
            EventHideItem?.Invoke(component, rectIndex);
            component.gameObject.SetActive(false);
            componentPool.Push(component);
        }

        private void UpdateScrollContentPosition(float targetIndex)
        {
            var targetPosition = prefabRectTransform.rect.height * (targetIndex + 1);
            var rectHeight = scrollRect.content.rect.height - targetPosition;
            scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, rectHeight);
        }
        
        private void TryKillSequence()
        {
            if (sequence != null)
            {
                sequence.Kill();
                sequence = null;
            }
        }
        
        private void OnDestroy()
        {
            TryKillSequence();
        }
    }
}