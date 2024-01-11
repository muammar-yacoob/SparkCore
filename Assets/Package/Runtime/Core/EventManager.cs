﻿using System;
using System.Collections.Generic;

namespace SparkCore.Runtime.Core
{
    public sealed class EventManager
    {
        private static readonly Lazy<EventManager> lazy = new(() => new EventManager());
        public static EventManager Instance => lazy.Value;

        private readonly Dictionary<Type, List<Delegate>> eventDictionary = new Dictionary<Type, List<Delegate>>();

        public void SubscribeEvent<T>(Action<T> action)
        {
            Type type = typeof(T);
            if (!eventDictionary.ContainsKey(type))
            {
                eventDictionary[type] = new List<Delegate>();
            }
            eventDictionary[type].Add(action);
        }

        public void UnsubscribeEvent<T>(Action<T> action)
        {
            Type type = typeof(T);
            if (eventDictionary.TryGetValue(type, out var existing))
            {
                existing.Remove(action);
                if (existing.Count == 0)
                {
                    eventDictionary.Remove(type);
                }
            }
        }

        public void PublishEvent<T>(T eventType)
        {
            Type type = typeof(T);
            if (eventDictionary.TryGetValue(type, out var thisEvent))
            {
                for (int i = thisEvent.Count - 1; i >= 0; i--)
                {
                    var del = thisEvent[i];
                    if (del is Action<T> action)
                    {
                        action?.Invoke(eventType);
                    }
                }
            }
        }
    }
}