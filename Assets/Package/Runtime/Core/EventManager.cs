using System;
using System.Collections.Generic;
using System.Linq;

namespace SparkCore.Runtime.Core
{
    /// <summary>
    ///  Handles event subscriptions and publishing.
    /// </summary>
    public sealed class EventManager
    {
        private static readonly Lazy<EventManager> lazy = new(() => new EventManager());
        /// <summary>
        /// Singleton instance of the EventManager.
        /// </summary>
        public static EventManager Instance => lazy.Value;

        private readonly Dictionary<Type, List<Delegate>> eventDictionary = new Dictionary<Type, List<Delegate>>();

        /// <summary>
        /// Subscribe to an event.
        /// </summary>
        /// <param name="action">Action to be invoked when the event is published. </param>
        /// <typeparam name="T">Event type </typeparam>
        public void SubscribeEvent<T>(Action<T> action)
        {
            Type type = typeof(T);
            if (!eventDictionary.ContainsKey(type))
            {
                eventDictionary[type] = new List<Delegate>();
            }
            eventDictionary[type].Add(action);
        }

        /// <summary>
        /// Unsubscribe from an event.
        /// </summary>
        /// <param name="action">Action to unsubscribe from. </param>
        /// <typeparam name="T">Event type </typeparam>
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

        /// <summary>
        /// Publish an event.
        /// </summary>
        /// <param name="eventType">Event to publish. </param>
        /// <typeparam name="T">Event type </typeparam>
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
        
        /// <summary>
        /// Gets the list of subscribers for a specific event type.
        /// </summary>
        /// <typeparam name="T">Event type.</typeparam>
        /// <returns>List of subscribers for the event.</returns>
        public List<Delegate> GetSubscribers<T>()
        {
            Type baseType = typeof(MonoEvent);
            var allSubscribers = new List<Delegate>();

            foreach (var entry in eventDictionary)
            {
                // Check if the key is a subclass of SceneEvent
                if (baseType.IsAssignableFrom(entry.Key))
                {
                    allSubscribers.AddRange(entry.Value);
                }
            }
            return allSubscribers;
        }
        
        /// <summary>
        /// Returns the list of subscribers for a specific event type.
        /// </summary>
        public List<Delegate> GetCustomSubscribers<T>()
        {
            Type type = typeof(T);
            return eventDictionary.TryGetValue(type, out var subscribers) ? subscribers.ToList() : new List<Delegate>();
        }
    }
}