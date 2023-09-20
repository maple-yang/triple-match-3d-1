using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Components;
using Game.Core.Configurations;
using Game.Data.Models;
using Game.Modules.AudioManager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Core.Controllers
{
    public class GameEffectController : IDisposable
    {
        public GameEffectController(
            ItemsInventory itemsInventory,
            IAudioController audioController,
            ItemsParameters itemsParameters)
        {
            this.itemsInventory = itemsInventory;
            this.audioController = audioController;
            this.itemsParameters = itemsParameters;
        }

        private readonly ItemsInventory itemsInventory;
        private readonly ItemsParameters itemsParameters;
        private readonly IAudioController audioController;
        private CompositeDisposable compositeDisposable;
        private ComponentPool<byte, ParticleSystem> mergeEffectComponentPool;
        private List<ParticleSystem> mergeParticleSystem;

        [Inject]
        private void Initialization()
        {
            mergeParticleSystem = new List<ParticleSystem>();
            mergeEffectComponentPool = new ComponentPool<byte, ParticleSystem>();
            compositeDisposable = new CompositeDisposable();
            
            itemsInventory.EventItemAddedToFreeSlot.Subscribe(OnItemAddedToFreeSlot).AddTo(compositeDisposable);
            itemsInventory.EventInventoryFull.Subscribe(OnInventoryFull).AddTo(compositeDisposable);
            itemsInventory.EventMergeItems.Subscribe(OnMergeItems).AddTo(compositeDisposable);
        }

        private void OnItemAddedToFreeSlot(string itemId)
        {
            audioController.TryPlaySound(AudioNameData.TAKE_ITEM_SOUND);
        }
        
        private void OnInventoryFull(Unit unit)
        {
            
        }
        
        private void OnMergeItems(ItemSlot[] itemSlots)
        {
            audioController.TryPlaySound(AudioNameData.MERGE_ITEM_SOUND);

            var notActiveParticles = mergeParticleSystem.Where(m => m.isStopped).ToList();
            if (notActiveParticles.Count > 0)
            {
                foreach (var particleSystem in notActiveParticles)
                {
                    mergeParticleSystem.Remove(particleSystem);
                    mergeEffectComponentPool.Push(particleSystem);
                }
            }
            
            var particle = mergeEffectComponentPool.Pop(itemsParameters.MergeParticleSystem);
            particle.transform.SetParent(itemSlots[1].Slot);
            particle.transform.localPosition = Vector3.zero;
            particle.Play();
            mergeParticleSystem.Add(particle);
        }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }
    }
}