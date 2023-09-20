using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Core.Components;
using Game.Core.Configurations;
using Game.Data.Models;
using Game.Systems.TimeSystem;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Core.Controllers
{
    public class BoosterController : IBoostersController
    {
        public BoosterController
        (
            BoostersParameters boostersParameters,
            GameDataModel gameDataModel,
            ItemsContainer itemsContainer,
            ItemsInventory itemsInventory,
            ITimer gameTimer
        )
        {
            this.boostersParameters = boostersParameters;
            this.gameDataModel = gameDataModel;
            this.itemsInventory = itemsInventory;
            this.itemsContainer = itemsContainer;
            this.gameTimer = gameTimer;
        }

        private readonly BoostersParameters boostersParameters;
        private readonly GameDataModel gameDataModel;
        private readonly ItemsInventory itemsInventory;
        private readonly ItemsContainer itemsContainer;
        private readonly ITimer gameTimer;

        private readonly CompositeDisposable compositeDisposables = new CompositeDisposable();

        private bool isShuffleCooldown = false;
        private bool isFreezingCooldown = false;

        private BoosterType boosterType;
        
        public ISubject<BoosterType> EventBoosterActionStart { get; private set; }
        public ISubject<BoosterType> EventBoosterActionFinished { get; private set; }

        [Inject]
        private void Initialization()
        {
            EventBoosterActionStart = new Subject<BoosterType>();
            EventBoosterActionFinished = new Subject<BoosterType>();
        }

        public void UseBooster(BoosterType boosterType, bool isFree)
        {
            this.boosterType = boosterType;
            
            // TODO Make from strategy pattern
            bool success = false;
            switch (boosterType)
            {
                case BoosterType.Magnet:
                    if (TryGetBestItemIDForMagnetBooster(out string id, out int count))
                    {
                        success = true;
                        UseMagnetBooster(id, count);
                    }
                    break;
                case BoosterType.Cancel:
                    if (itemsInventory.TryRemoveAndGetLastItem(out var item))
                    {
                        success = true;
                        UseCancelBooster(item).Forget();
                    }
                    break;
                case BoosterType.Shuffle:
                    if (isShuffleCooldown == false)
                    {
                        success = true;
                        UseShuffleBooster();
                    }
                    break;
                case BoosterType.Freezing:
                    if (isFreezingCooldown == false)
                    {
                        success = true;
                        UseFreezeBooster();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(boosterType), boosterType, null);
            }

            if (success && !isFree)
            {
                var data = gameDataModel.GetBoosterData(boosterType);
                data.Counter.Value--;
                gameDataModel.Save();
                EventBoosterActionStart.OnNext(boosterType);
            }   
            else
            {
                EventBoosterActionFinished.OnNext(boosterType);
            }
        }
        
        public void Dispose()
        {
            compositeDisposables.Dispose();
        }

        private async void UseMagnetBooster(string itemsID, int count)
        {
            var items = itemsContainer.GetItems(itemsID, count);
            var tasks = new List<UniTask>();
            foreach (var item in items)
            {
                itemsContainer.RemoveItem(item);
                var task = itemsInventory.AddItemToFreeSlot(item);
                tasks.Add(task);
            }

            await UniTask.WhenAll(tasks);
            
            EventBoosterActionFinished.OnNext(boosterType);
        }

        private async UniTaskVoid UseCancelBooster(ItemView cancelItem)
        {
            cancelItem.StopIdleAnimation();
            itemsContainer.AddItem(cancelItem);
            
            var randomPoint = itemsContainer.GetRandomPointInContainer();
            var yPoint = itemsContainer.Items.Max(i => i.Position.y);
            randomPoint = new Vector3(randomPoint.x, yPoint, randomPoint.z);
            
            var rotationTask = cancelItem.RotateItem(Quaternion.Euler((Vector3.up + Vector3.back) * 90), boostersParameters.JumpDuration * 2, boostersParameters.JumpEase);
            var task1 = cancelItem.JumpItem(randomPoint, boostersParameters.JumpHeight, boostersParameters.JumpDuration, boostersParameters.JumpEase);
            var task2 = cancelItem.ScaleItem(1, boostersParameters.JumpDuration * 0.25f, boostersParameters.JumpEase);

            cancelItem.SetInteractable(true);
            cancelItem.SetActivePhysics(true);
            cancelItem.SetEnableCollider(true);
            
            await UniTask.WhenAll(task1, task2, rotationTask);

            gameDataModel.EventAddItemToField.OnNext(cancelItem.ID);
            EventBoosterActionFinished.OnNext(boosterType);
        }

        private void UseShuffleBooster()
        {
            var items = itemsContainer.Items;
            foreach (var item in items)
            {
                var randomForce = Random.Range(boostersParameters.ShuffleForceMin, boostersParameters.ShuffleForceMax);
                var randomTorqueX = Random.Range(boostersParameters.ShuffleAngularForceMin, boostersParameters.ShuffleAngularForceMax);
                var randomTorqueY = Random.Range(boostersParameters.ShuffleAngularForceMin, boostersParameters.ShuffleAngularForceMax);
                var randomTorqueZ = Random.Range(boostersParameters.ShuffleAngularForceMin, boostersParameters.ShuffleAngularForceMax);
                
                var randomTorque = new Vector3(randomTorqueX, randomTorqueY, randomTorqueZ);
                var force = Vector3.up * randomForce;
                
                item.Rigidbody.AddForce(force, ForceMode.VelocityChange);
                item.Rigidbody.AddTorque(randomTorque, ForceMode.VelocityChange);
            }

            isShuffleCooldown = true;
            var timer = Observable.Timer(TimeSpan.FromSeconds(boostersParameters.ShuffleCooldown)).Subscribe(_ =>
            {
                isShuffleCooldown = false;
                EventBoosterActionFinished.OnNext(boosterType);
            }).AddTo(compositeDisposables);
        }

        private void UseFreezeBooster()
        {
            isFreezingCooldown = true;
            gameTimer.Pause();
            var timer = Observable.Timer(TimeSpan.FromSeconds(boostersParameters.FreezingDuration)).Subscribe(_ =>
            {
                isFreezingCooldown = false;
                gameTimer.Continue();
                EventBoosterActionFinished.OnNext(boosterType);
            }).AddTo(compositeDisposables);
        }

        private bool TryGetBestItemIDForMagnetBooster(out string id, out int count)
        {
            id = string.Empty;
            count = 0;
            
            var goals = gameDataModel.RemainingItemGoalData;
            var itemsInInventory = itemsInventory.GetItemsInSlots();
            var goalItemsInInventory = itemsInInventory.Where(t => goals.ContainsKey(t.ID)).ToArray();
            if (goalItemsInInventory.Length > 0)
            {
                foreach (var item in goalItemsInInventory)
                {
                    var itemID = item.ID;
                    var goalItems = goalItemsInInventory.Where(t => t.ID == itemID).ToArray();
                    var itemsNeed = goals[itemID] > 3 ? 3 - goalItems.Length : goals[itemID];
                    var itemsInContainer = itemsContainer.GetItemsCount(itemID);

                    if (itemsInventory.FreeSlotsCount >= itemsNeed && itemsInContainer >= itemsNeed)
                    {
                        id = itemID;
                        count = itemsNeed;
                        return true;
                    }
                }
            }
            else
            {
                var needItemPair = goals.OrderBy(t => t.Value).Reverse();
                foreach (var item in needItemPair)
                {
                    var needItemId = item.Key;
                    var needItemsCount = item.Value > 3 ? 3 : item.Value;
                    var itemsInContainer = itemsContainer.GetItemsCount(needItemId);

                    if (itemsInContainer >= needItemsCount)
                    {
                        id = needItemId;
                        count = needItemsCount;
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}