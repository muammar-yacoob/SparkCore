using System;
using System.Collections.Generic;

namespace SparkCore.Runtime.Core
{
    public sealed class EventManager
    {
        private static readonly Lazy<EventManager> lazy = new Lazy<EventManager>(() => new EventManager());
        public static EventManager Instance => lazy.Value;
        private readonly Dictionary<Type, Action<object>> eventDictionary;

        private EventManager()
        {
            eventDictionary = new Dictionary<Type, Action<object>>();
        }

        public void PublishEvent<T>(T eventType)
        {
            Type type = typeof(T);
        
            if (eventDictionary.ContainsKey(type))
            {
                eventDictionary[type]?.Invoke(eventType);
            }
        }

        public void SubscribeEvent<T>(Action<T> action)
        {
            Type type = typeof(T);

            if (eventDictionary.ContainsKey(type))
            {
                eventDictionary[type] = (Action<object>) Delegate.Combine(eventDictionary[type], (Action<object>) (x => action((T) x)));
            }
            else
            {
                eventDictionary[type] = (x => action((T) x));
            }
        }
    }
}