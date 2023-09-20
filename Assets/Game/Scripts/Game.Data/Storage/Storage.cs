using System;
using Game.Data.Models;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

namespace Game.Data.Storage
{
    public class Storage : IStorage
    {
        public IObservable<bool> HasProperty(string propertyName)
        {
            var propertyValue = PlayerPrefs.GetString(propertyName, string.Empty);
            return Observable.Return(!string.IsNullOrEmpty(propertyValue));
        }

        public IObservable<bool> Load<T>(T loadingObject, string keyName) where T : IPersistence
        {
            bool result;

            try
            {
                var stringValue = PlayerPrefs.GetString(keyName);
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };

                var persistence = JsonConvert.DeserializeObject(stringValue, loadingObject.Persistence.GetType(), settings);
                loadingObject.Persistence = persistence;
                result = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error on load {loadingObject} by key '{keyName}': {e.Message}\r\n{e.StackTrace}");
                result = false;
            }

            return Observable.Return(result);
        }

        public void Save<T>(T savingObject, string keyName) where T : IPersistence
        {
            var stringValue = JsonConvert.SerializeObject(savingObject.Persistence, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            PlayerPrefs.SetString(keyName, stringValue);
        }
    }
}