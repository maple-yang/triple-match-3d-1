using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Core.Components
{
    public class ItemsContainer : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private Transform itemsRoot;

        [SerializeField]
        private BoxCollider levelBounds;
        
        [SerializeField]
        private List<BoxCollider> levelSpaceBounds;

        private readonly List<ItemView> items = new List<ItemView>();

        public IReadOnlyCollection<ItemView> Items => items;
        public Bounds LevelBounds => levelBounds.bounds;

        public void AddItem(ItemView itemView)
        {
            items.Add(itemView);
            itemView.SetParent(itemsRoot);
        }

        public void AddItemsRange(List<ItemView> itemViews)
        {
            itemViews.ForEach(t => t.SetParent(itemsRoot));
            items.AddRange(itemViews);
        }

        public void RemoveItem(ItemView itemView)
        {
            if (items.Contains(itemView) == false)
            {
                Debug.LogError($"No item \'{itemView.name}\' in container!");
                return;
            }

            items.Remove(itemView);
        }

        public void Dispose()
        {
            // TODO Add pool
            items.ForEach(t => Destroy(t.gameObject));
            items.Clear();
        }

        public Vector3 GetRandomPointInContainer()
        {
            var randomX = Random.Range(LevelBounds.min.x, LevelBounds.max.x);
            var randomY = Random.Range(LevelBounds.min.y, LevelBounds.max.y);
            var randomZ = Random.Range(LevelBounds.min.z, LevelBounds.max.z);

            var random = new Vector3(randomX, randomY, randomZ);
            return random;
        }

        public List<ItemView> GetItems([NotNull]string itemsId, int itemsCount)
        {
            var selectedItems = items.Where(t => t.ID == itemsId).ToArray();
            return selectedItems.Take(itemsCount).ToList();
        }

        public int GetItemsCount(string itemsID)
        {
            return items.Count(t => t.ID == itemsID);
        }

        public void SetInteractableItems(bool isInteractable)
        {
            items.ForEach(it => it.SetInteractable(isInteractable));
        }
        
        public void SetLevelSpaceBounds(CameraConstantWidth cameraConstantWidth, Camera mainCamera)
        {
            // позиция боксов переднего и заднего
            var offset = mainCamera.orthographicSize - cameraConstantWidth.DefaultInitialSize;
            var forwardBound = levelSpaceBounds[4];
            var backBound = levelSpaceBounds[5];
            forwardBound.center += new Vector3(0, 0, offset);
            backBound.center -= new Vector3(0, 0, offset);
            
            // размер боксов слева и справа
            var rightBound = levelSpaceBounds[2];
            var leftBound = levelSpaceBounds[3];
            var fieldHeightSize = Math.Abs(forwardBound.center.z) + Math.Abs(backBound.center.z) + (forwardBound.size.z / 2) + (backBound.size.z / 2);
            rightBound.size = new Vector3(rightBound.size.x, rightBound.size.y, fieldHeightSize);
            leftBound.size = new Vector3(leftBound.size.x, leftBound.size.y, fieldHeightSize);
            var fieldHeightCenter = ((forwardBound.center.z + backBound.center.z) / 2);
            rightBound.center = new Vector3(rightBound.center.x, rightBound.center.y, fieldHeightCenter);
            leftBound.center = new Vector3(leftBound.center.x, leftBound.center.y, fieldHeightCenter);
            
            // размер боксов потолка и пола
            var downBound = levelSpaceBounds[0];
            var upBound = levelSpaceBounds[1];
            var fieldWightSize = Math.Abs(rightBound.center.x) + Math.Abs(leftBound.center.x) + (rightBound.size.x / 2) + (leftBound.size.x / 2);
            downBound.size = new Vector3(fieldWightSize, downBound.size.y, fieldHeightSize);
            upBound.size = new Vector3(fieldWightSize, upBound.size.y, fieldHeightSize);
            var fieldWightCenter = ((rightBound.center.x + leftBound.center.x) / 2);
            downBound.center = new Vector3(fieldWightCenter, downBound.center.y, fieldHeightCenter);
            upBound.center = new Vector3(fieldWightCenter, upBound.center.y, fieldHeightCenter);

            //размер бокса для спавна обьектов
            fieldHeightSize -= forwardBound.size.z + backBound.size.z + 1f;
            fieldWightSize -= rightBound.size.x + leftBound.size.x + 1f;
            levelBounds.size = new Vector3(fieldWightSize, levelBounds.size.y, fieldHeightSize);
            levelBounds.center = new Vector3(fieldWightCenter, levelBounds.center.y, fieldHeightCenter);
        }

        public Vector3 GetBackBoundPosition()
        {
            return levelSpaceBounds[5].center;
        }
    }
}