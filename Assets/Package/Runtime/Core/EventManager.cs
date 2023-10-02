using System;
using System.Collections.Generic;

public sealed class EventManager
{
    private static readonly Lazy<EventManager> lazy = new Lazy<EventManager>(() => new EventManager());
    
    public static EventManager Instance => lazy.Value;

    private readonly Dictionary<Type, Action<object>> eventDictionary = new Dictionary<Type, Action<object>>();

    private EventManager() { }

    /// <summary>
    /// Publishes an event of the specified type.
    /// </summary>
    /// <typeparam name="T">Event type.</typeparam>
    /// <param name="eventType">Event data.</param>
    public void PublishEvent<T>(T eventType)
    {
        Type type = typeof(T);
        if (eventDictionary.ContainsKey(type)) eventDictionary[type]?.Invoke(eventType);
    }

    /// <summary>
    /// Subscribes to an event of the specified type.
    /// </summary>
    /// <typeparam name="T">Event type.</typeparam>
    /// <param name="action">Callback function to invoke when the event is published.</param>
    public void SubscribeEvent<T>(Action<T> action)
    {
        Type type = typeof(T);
        Action<object> objAction = x => action((T)x);
        eventDictionary[type] = eventDictionary.ContainsKey(type) ? eventDictionary[type] + objAction : objAction;
    }

    /// <summary>
    /// Unsubscribes from an event of the specified type.
    /// </summary>
    /// <typeparam name="T">Event type.</typeparam>
    /// <param name="action">Callback function to remove from event.</param>
    public void UnsubscribeEvent<T>(Action<T> action)
    {
        Type type = typeof(T);
        Action<object> objAction = x => action((T)x);

        if (eventDictionary.ContainsKey(type))
        {
            eventDictionary[type] -= objAction;
            if (eventDictionary[type] == null) eventDictionary.Remove(type);
        }
    }
}