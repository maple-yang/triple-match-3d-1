using System;
using Game.Data.Command;
using UnityEngine;
using Zenject;

namespace Game.Data.Models
{
    public abstract class DataModel : IPersistence
    {
        [Inject]
        private SaveDataCommand.Factory _saveDataCommandFactory;
        [Inject]
        private LoadDataCommand.Factory _loadDataCommandFactory;

        public bool JustCreate { get; set; } = false;
        public abstract object Persistence { get; set; }
        
        public static void BindData<T>(DiContainer container)
            where T : DataModel, new()
        {
            var dataModel = new T();
            container.Bind<T>().FromInstance(dataModel).AsSingle();
            container.Bind<DataModel>().FromInstance(dataModel);
            
            // For inject inner commands
            container.QueueForInject(dataModel);
        }
        
        public static void BindData(DiContainer container, Type type)
        {
            var dataModel = Activator.CreateInstance(type);
            container.Bind(type).FromInstance(dataModel).AsSingle();
            container.Bind<DataModel>().FromInstance(dataModel as DataModel);
            
            // For inject inner commands
            container.QueueForInject(dataModel);
        }
        
        /// <summary>
        /// Start save UserData
        /// </summary>
        /// <returns></returns>
        public virtual SaveDataCommand Save()
        {
            var cmd = _saveDataCommandFactory.Create(this);
            cmd.Execute();
            return cmd;
        }

        /// <summary>
        /// Start save UserData
        /// </summary>
        /// <returns></returns>
        public virtual LoadDataCommand Load()
        {
            var cmd = GetLoader();
            cmd.Execute();
            
            if (cmd.IsCompleted) OnDataLoaded();
            else cmd.Completed += OnLoadCommandComplete;
            
            return cmd;
        }

        public LoadDataCommand GetLoader()
        {
            return _loadDataCommandFactory.Create(this);
        }

        protected virtual void OnDataLoaded()
        {
        }
        
        private void OnLoadCommandComplete(ICommand command, bool result)
        {
            if(result) OnDataLoaded();
        }
    }

    public class DataModel<T> : DataModel
        where T : new()
    {
        public T Data { get; private set; } = new T();

        public override object Persistence
        {
            get => Data;
            set => Data = (T)value;
            //Data.AfterLoad();
        }
    }
}