using System;
using System.Linq;
using Game.Data.Command;
using Game.Data.Models;
using Game.Data.Storage;
using Zenject;

namespace Game.Data.Installers
{
    public class DataInstaller : Installer<DataInstaller>
    {
        public override void InstallBindings()
        {
            // install app data factory
            Container.BindFactory<IPersistence, LoadDataCommand, LoadDataCommand.Factory>();
            Container.BindFactory<IPersistence, SaveDataCommand, SaveDataCommand.Factory>();
            
            Container.Bind<IStorage>().To<Storage.Storage>().AsSingle();

            // install app data
            var autoBindType = typeof(IAutoBindDataModel);
            var targAutoDatas = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && autoBindType.IsAssignableFrom(t));
            
            foreach (var targAutoData in targAutoDatas)
            {
                DataModel.BindData(Container, targAutoData);
            }
        }
    }
}