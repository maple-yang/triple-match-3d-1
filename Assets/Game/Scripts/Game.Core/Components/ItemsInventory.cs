using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Core.Configurations;
using Game.Data.Models;
using Game.Modules.AudioManager;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Core.Components
{
    public class ItemsInventory : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private List<ItemSlot> inventorySlots;
        
        [Inject]
        private ItemsContainer itemsContainer;

        [Inject]
        private ItemsParameters itemsParameters;

        public bool IsInventoryFull { get; set; } = false;
        public int FreeSlotsCount => inventorySlots.Count(t => t.Item == null);
        public ISubject<Unit> EventInventoryFull { get; private set; }
        public ISubject<string> EventItemAddedToFreeSlot { get; private set; }
        public ISubject<ItemSlot[]> EventMergeItems { get; private set; }

        [Inject]
        private void Initialization()
        {
            EventInventoryFull = new Subject<Unit>();
            EventItemAddedToFreeSlot = new Subject<string>();
            EventMergeItems = new Subject<ItemSlot[]>();
        }
        
        public async UniTask AddItemToFreeSlot(ItemView itemView)
        {
            if (IsInventoryFull)
            {
                Debug.LogError("No free slot in inventory!");
                return;
            }

            var animationTasks = new List<UniTask>();
            var sameSlot = inventorySlots.LastOrDefault(t => t.Item != null && t.Item.ID == itemView.ID);
            if (sameSlot != null)
            {
                var moveIndex = inventorySlots.IndexOf(sameSlot) + 1;
                var slot = inventorySlots[moveIndex];
                
                var task1 = MoveItemsToRight(moveIndex);
                var task2 = MoveItemToSlot(itemView, slot, false);
                
                animationTasks.Add(task1);
                animationTasks.Add(task2);
            }
            else
            {
                var slot = inventorySlots.FirstOrDefault(t => t.Item == null);
                if (slot == null)
                {
                    return;
                }

                var task = MoveItemToSlot(itemView, slot, false);
                animationTasks.Add(task);
            }

            var rotationTask = itemView.RotateItem(Quaternion.identity, itemsParameters.MoveToInventoryDuration, itemsParameters.MoveToInventoryEase);
            var scaleTask = itemView.ScaleItem(itemsParameters.ItemsInSlotSize, itemsParameters.MoveToInventoryDuration, itemsParameters.MoveToInventoryEase);

            animationTasks.Add(rotationTask);
            animationTasks.Add(scaleTask);

            itemView.SetActivePhysics(false);
            itemView.SetEnableCollider(false);
            
            EventItemAddedToFreeSlot.OnNext(itemView.ID);

            IsInventoryFull = inventorySlots.Any(t => t.Item == null) == false;

            await UniTask.WhenAll(animationTasks);
            await TryMergeItems(itemView.ID);
            
            itemView.SetInteractable(false);

            IsInventoryFull = inventorySlots.Any(t => t.Item == null) == false;
            if (IsInventoryFull)
            {
                EventInventoryFull.OnNext(Unit.Default);
            }
        }
        
        public void Dispose()
        {
            inventorySlots.ForEach(t =>
            {
                if (t.Item != null)
                {
                    Destroy(t.Item.gameObject);
                }
                t.Item = null;
            });
            IsInventoryFull = false;
        }

        public bool TryRemoveAndGetLastItem(out ItemView itemView)
        {
            var slot = inventorySlots.LastOrDefault(t => t.Item != null);
            if (slot != null)
            {
                itemView = slot.Item;
                slot.Item = null;
                return true;
            }

            itemView = null;
            return false;
        }

        public List<ItemView> GetItemsInSlots()
        {
            return inventorySlots.Select(t => t.Item).Where(t => t != null).ToList();
        }

        private async UniTask TryMergeItems(string itemsID)
        {
            var slots = inventorySlots.Where(t => t.Item != null && t.Item.ID == itemsID).ToArray();
            var items = slots.Select(t => t.Item).ToArray();
            
            if (slots.Length == 3)
            {
                EventMergeItems.OnNext(slots);
                
                for (var i = 0; i < slots.Length; i++)
                {
                    var slot = slots[i];
                    slot.Item = null;
                }
                
                await PlayMergeItemsAnimations(items);
                items.ForEach(t => Destroy(t.gameObject));
                await MoveItemsToLeft();
            }
        }

        private async UniTask PlayMergeItemsAnimations(ItemView[] items)
        {
            var mergeCenter = Vector3.zero;
            var animationsTasks = new List<UniTask>();

            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                mergeCenter += item.Position;
            }

            mergeCenter /= items.Length;

            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var task = item.MoveItem(mergeCenter, itemsParameters.MergeDuration, itemsParameters.MergeEase);
                animationsTasks.Add(task);
            }
            
            await UniTask.WhenAll(animationsTasks);
            
            animationsTasks.Clear();
            
            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var task = item.ScaleItem(itemsParameters.MergeScale, itemsParameters.MergeScaleDuration, itemsParameters.MergeScaleEase);
                animationsTasks.Add(task);
            }
            
            await UniTask.WhenAll(animationsTasks);
        }

        private async UniTask MoveItemToSlot(ItemView itemView, ItemSlot itemSlot, bool inInventory)
        {
            itemSlot.Item = itemView;

            var duration = inInventory ? itemsParameters.MoveInInventoryDuration : itemsParameters.MoveToInventoryDuration;
            var ease = inInventory ? itemsParameters.MoveInInventoryEase : itemsParameters.MoveToInventoryEase;

            await itemView.MoveItem(itemSlot.Slot.position, duration, ease);
            itemView.PlayIdleAnimation(itemsParameters.IdleAnimationAmplitude, itemsParameters.IdleAnimationDuration, itemsParameters.IdleAnimationEase);
        }

        private async UniTask MoveItemsToRight(int startIndex)
        {
            ItemView nextItem = null;
            var tasks = new List<UniTask>();
            for (int i = startIndex; i < inventorySlots.Count; i++)
            {
                var currentSlot = inventorySlots[i];
                var item = currentSlot.Item;
                currentSlot.Item = null;
                
                if (nextItem != null)
                {
                    var task = MoveItemToSlot(nextItem, currentSlot, true);
                    tasks.Add(task);
                }

                nextItem = item;
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask MoveItemsToLeft()
        {
            var animationTasks = new List<UniTask>();
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                var slot = inventorySlots[i];
                if (slot.Item == null)
                {
                    for (int index = i + 1; index < inventorySlots.Count; index++)
                    {
                        var nextSlot = inventorySlots[index];
                        if (nextSlot.Item != null)
                        {
                            var item = nextSlot.Item;
                            nextSlot.Item = null;
                            var task = MoveItemToSlot(item, slot, true);
                            animationTasks.Add(task);
                            break;
                        }
                    }
                }
            }

            await UniTask.WhenAll(animationTasks);
        }

        public void SetLocalPosition(Vector3 localPosition)
        {
            transform.localPosition = localPosition;
        }
    }
    
    [Serializable]
    public class ItemSlot
    {
        [field: SerializeField]
        public Transform Slot { get; private set; }
        public ItemView Item { get; set; }
    }
}