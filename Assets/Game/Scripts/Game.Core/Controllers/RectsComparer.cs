using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Core.Controllers
{
    public class RectsComparer : IDisposable
    {
        public event Action<int, Rect> EventRectShow = delegate { };
        public event Action<int, Rect> EventRectHide = delegate { };

        private ScrollRect scrollRect;
        private Rect[] childrenRects;
        private HashSet<int> shownRectsHashSet = new HashSet<int>();
    
        public void SetScrollRects(ScrollRect scrollRect, IList<Rect> childrenRects)
        {
            this.scrollRect = scrollRect;
            this.childrenRects = childrenRects.ToArray();

            shownRectsHashSet.Clear();

            CheckRects();

            this.scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
            this.scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        private void OnScrollValueChanged(Vector2 scrollValue)
        {
            CheckRects();
        }

        private void CheckRects()
        {
            Rect viewPortRectWithOffset = scrollRect.viewport.rect;
            viewPortRectWithOffset.yMin -= 1000;
            viewPortRectWithOffset.yMax += 500;
            Rect worldViewportRect = CalculateWorldRect(viewPortRectWithOffset, scrollRect.viewport);

            for (int i = 0, l = childrenRects.Length; i < l; i++)
            {
                Rect childRect = childrenRects[i];
                Rect worldChildRect = CalculateWorldRect(childRect, scrollRect.content);

                if (worldViewportRect.Overlaps(worldChildRect))
                {
                    if (!shownRectsHashSet.Contains(i))
                    {
                        shownRectsHashSet.Add(i);

                        EventRectShow?.Invoke(i, childRect);
                    }
                }
                else
                {
                    if (shownRectsHashSet.Contains(i))
                    {
                        shownRectsHashSet.Remove(i);

                        EventRectHide?.Invoke(i, childRect);
                    }
                }
            }
        }

        public void RemoveRect(int rectIndex)
        {
            List<Rect> childrenRectsList = new List<Rect>(childrenRects);
            childrenRectsList.RemoveAt(rectIndex);

            shownRectsHashSet.Remove(rectIndex);
            DecreaseIndexes(rectIndex + 1);

            childrenRects = childrenRectsList.ToArray();

            CheckRects();
        }

        private void DecreaseIndexes(int fromIndex)
        {
            int[] rectsIndexes = new int[shownRectsHashSet.Count];
            shownRectsHashSet.CopyTo(rectsIndexes);

            for (int i = 0, l = rectsIndexes.Length; i < l; i++)
            {
                if (rectsIndexes[i] >= fromIndex)
                    rectsIndexes[i]--;
            }

            shownRectsHashSet = new HashSet<int>(rectsIndexes);
        }

        private Rect CalculateWorldRect(Rect rect, Transform parentTransform)
        {
            Rect worldRect = new Rect();
            worldRect.min = parentTransform.TransformPoint(rect.min);
            worldRect.max = parentTransform.TransformPoint(rect.max);

            return worldRect;
        }

        public void Dispose()
        {
            if (scrollRect)
            {
                scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
            }

            childrenRects = null;
        }
    }
}
