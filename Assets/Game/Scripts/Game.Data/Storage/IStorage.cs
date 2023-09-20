using System;
using Game.Data.Models;

namespace Game.Data.Storage
{
    public interface IStorage
    {
        IObservable<bool> HasProperty(string propertyName);
        IObservable<bool> Load<T>(T loadingObject, string keyName) where T : IPersistence;
        void Save<T>(T savingObject, string keyName) where  T : IPersistence;
    }
}