using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Modules.TutorModule.View
{
    public class ShadowRects : MonoBehaviour
    {
        [SerializeField] private RectTransform unmaskRectTransformPrefab = null;
        [SerializeField] private UnmaskRaycastFilter unmaskRaycastFilter = null;
        [SerializeField] private List<Sprite> unmaskSprite = null;
        
        private Dictionary<RectTransform, RectTransform> unmasksDictionaty = new Dictionary<RectTransform, RectTransform>();
        private Dictionary<int, RectTransform> unmasksDictionatyIndex = new Dictionary<int, RectTransform>();
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void AddUnmask(RectTransform targetUnmaskRectTransform, Vector2 margin = default, int indexUnmask = 0, int spriteIndex = 0)
        {
            RectTransform unmaskRectTransform = Instantiate(unmaskRectTransformPrefab, unmaskRectTransformPrefab.parent);
            unmaskRectTransform.position = targetUnmaskRectTransform.position;
            unmaskRectTransform.pivot = targetUnmaskRectTransform.pivot;
            unmaskRectTransform.sizeDelta = targetUnmaskRectTransform.sizeDelta + (margin == default ? Vector2.zero : 2f * margin);
            unmaskRectTransform.gameObject.SetActive(true);
            unmaskRectTransform.SetSiblingIndex(0);
            
            Image unmaskImage = unmaskRectTransform.GetComponent<Image>();
            unmaskImage.sprite = unmaskSprite[spriteIndex];
            Unmask unmask = unmaskRectTransform.GetComponent<Unmask>();
            unmaskRaycastFilter.TargetUnmasks.Add(unmask);
        
            unmasksDictionaty[targetUnmaskRectTransform] = unmaskRectTransform;
            if (indexUnmask > 0)
            {
                unmasksDictionatyIndex[indexUnmask] = unmaskRectTransform;
                UpdateUnmask(null, margin, indexUnmask);
            }
        }
        
        public void RemoveUnmask(RectTransform targetUnmaskRectTransform)
        {
            if (unmasksDictionaty.ContainsKey(targetUnmaskRectTransform))
            {
                Unmask unmask = unmasksDictionaty[targetUnmaskRectTransform].GetComponent<Unmask>();
                unmaskRaycastFilter.TargetUnmasks.Remove(unmask);
                unmasksDictionaty[targetUnmaskRectTransform].gameObject.SetActive(false);
                unmasksDictionaty.Remove(targetUnmaskRectTransform);
            }
        }

        public void RemoveAllUnmask()
        {
            foreach(KeyValuePair<RectTransform, RectTransform> unmaskData in unmasksDictionaty)
            {
                unmaskData.Value.gameObject.SetActive(false);
            }
            foreach(KeyValuePair<int, RectTransform> unmaskData in unmasksDictionatyIndex)
            {
                unmaskData.Value.gameObject.SetActive(false);
            }
            unmasksDictionaty.Clear();
            unmasksDictionatyIndex.Clear();
        }
        
        public void UpdateUnmask(RectTransform refUnmaskRectTransform, Vector2 margin, int indexUnmask)
        {
            if (unmasksDictionatyIndex.ContainsKey(indexUnmask))
            {
                RectTransform unmaskRectTransform = unmasksDictionatyIndex[indexUnmask];
                if (refUnmaskRectTransform == null)
                {
                    unmaskRectTransform.gameObject.SetActive(false);
                    return;
                }
                unmaskRectTransform.gameObject.SetActive(true);
                unmaskRectTransform.position = refUnmaskRectTransform.position;
                unmaskRectTransform.pivot = refUnmaskRectTransform.pivot;
                unmaskRectTransform.sizeDelta = refUnmaskRectTransform.sizeDelta + (2f * margin);
            }
        }
    }
}