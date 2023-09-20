using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Game.Utils.Utils
{
    /// <summary>
    /// Can subscribe to UniRx(IObservable) and Unsubscribe by any key
    /// </summary>
    // TODO: think about using CompositeDisposable from UniRx
    public class SubscribersContainer
    {
        private readonly Dictionary<object, List<IDisposable>> m_subscribeMap =
            new Dictionary<object, List<IDisposable>>();
        
        public void Subscribe<T>(IObservable<T> reactProp, Action<T> callback, object subscriptionKey = null)
        {
            FixSubscriptionKey(ref subscriptionKey);
            if (!m_subscribeMap.TryGetValue(subscriptionKey, out var subscribers))
            {
                subscribers = new List<IDisposable>();
                m_subscribeMap[subscriptionKey] = subscribers;
            }
            var res = reactProp.Subscribe(callback);
            subscribers.Add(res);
        }

        public void FreeAllSubscribers()
        {
            foreach (var kvp in m_subscribeMap)
            {
                foreach (var subscriber in kvp.Value)
                {
                    subscriber.Dispose();
                }
                kvp.Value.Clear();
            }
            m_subscribeMap.Clear();
        }

        public void FreeSubscribers(object subscriptionKey = null)
        {
            FixSubscriptionKey(ref subscriptionKey);
            if (!m_subscribeMap.TryGetValue(subscriptionKey,  out var subscribers))
            {
                return;
            }
            foreach (var subscriber in subscribers)
            {
                subscriber.Dispose();
            }
            subscribers.Clear();
        }

        public bool IsEmpty()
        {
            if (m_subscribeMap.Count == 0)
            {
                return true;
            }

            if (m_subscribeMap.All(kvp => kvp.Value.Count == 0))
            {
                return true;
            }

            return false;
        }

        private void FixSubscriptionKey(ref object subscriptionKey)
        {
            if (subscriptionKey == null)
            {
                subscriptionKey = this;
            }
        }
    }
}