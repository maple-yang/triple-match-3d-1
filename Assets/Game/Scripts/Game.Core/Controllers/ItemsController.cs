using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Core.Components;
using Game.Core.Configurations;
using UniRx;
using UnityEngine;

namespace Game.Core.Controllers
{
    public class ItemsController : IDisposable
    {
        private readonly ItemsInventory itemsInventory;
        private readonly ItemsContainer itemsContainer;
        private readonly ItemsParameters itemsParameters;
        private readonly CompositeDisposable compositeDisposables;

        private bool isInteractable = true;
        private bool isDown = false;
        private ItemView lastPickedItem;

        public ItemsController(List<ItemView> itemsViews, ItemsInventory itemsInventory, ItemsContainer itemsContainer, ItemsParameters itemsParameters)
        {
            this.itemsInventory = itemsInventory;
            this.itemsContainer = itemsContainer;
            this.itemsParameters = itemsParameters;
            compositeDisposables = new CompositeDisposable();
            
            itemsViews.ForEach(t =>
            {
                t.EventItemPointerExit.Subscribe(OnItemExit).AddTo(compositeDisposables);
                t.EventItemPointerUp.Subscribe(OnItemUp).AddTo(compositeDisposables);
                t.EventItemPointerDown.Subscribe(OnItemDown).AddTo(compositeDisposables);
                t.EventItemPointerEnter.Subscribe(OnItemEnter).AddTo(compositeDisposables);
            });
        }

        public void Dispose()
        {
            isInteractable = false;
            compositeDisposables.Dispose();
        }

        private void OnItemEnter(ItemView itemView)
        {
            if (isInteractable == false || isDown == false)
            {
                return;
            }
            
            lastPickedItem = itemView;
            SetActiveOutline(itemView,true);
        }

        private void OnItemExit(ItemView itemView)
        {
            if (isInteractable == false || lastPickedItem == null)
            {
                return;
            }
            
            SetActiveOutline(itemView,false);

            lastPickedItem = null;
        }

        private void OnItemDown(ItemView itemView)
        {
            if (isInteractable == false)
            {
                return;
            }

            isDown = true;
            lastPickedItem = itemView;
            SetActiveOutline(itemView, true);
        }
        
        private void OnItemUp(ItemView itemView)
        {
            if (isInteractable == false || isDown == false)
            {
                return;
            }
            
            if (itemsInventory.IsInventoryFull == false && lastPickedItem != null)
            {
                itemsContainer.RemoveItem(lastPickedItem);
                AddItemToInventory(lastPickedItem).Forget();
                
                SetActiveOutline(lastPickedItem, false);
            }
            
            lastPickedItem = null;
            isDown = false;
        }

        private void SetActiveOutline(ItemView itemView, bool isActive)
        {
            if (isInteractable == false)
            {
                return;
            }

            itemView.MeshRenderer.gameObject.layer = isActive ? 3 : 0;

            //itemView.MeshRenderer.material.SetColor(itemView.OutlineColorMaterialProperty, itemsParameters.OutlineColor);
            //itemView.MeshRenderer.material.SetFloat(itemView.OutlineSizeMaterialProperty, isActive ? itemsParameters.OutlineSize : 0);
        }

        private async UniTaskVoid AddItemToInventory(ItemView itemView)
        {
            await itemsInventory.AddItemToFreeSlot(itemView);
        }
    }
}