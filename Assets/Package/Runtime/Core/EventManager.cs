using System;
using System.Collections.Generic;

public sealed class EventManager
{
    private static readonly Lazy<EventManager> lazy = new Lazy<EventManager>(() => new EventManager());
    public static EventManager Instance => lazy.Value;

    private readonly Dictionary<Type, Action<object>> eventDictionary = new();

    private EventManager() { }

    public void PublishEvent<T>(T eventType)
    {
        Type type = typeof(T);
        if (eventDictionary.TryGetValue(type, out var thisEvent))
        {
            thisEvent.Invoke(eventType);
        }
    }

    public void SubscribeEvent<T>(Action<T> action)
    {
        Type type = typeof(T);
        Action<object> objAction = x => action((T)x);

        if (!eventDictionary.TryGetValue(type, out var existing))
        {
            existing = null;
        }

        existing += objAction;
        eventDictionary[type] = existing;
    }

    public void UnsubscribeEvent<T>(Action<T> action)
    {
        Type type = typeof(T);
        Action<object> objAction = x => action((T)x);

        if (eventDictionary.TryGetValue(type, out var existing))
        {
            existing -= objAction;

            if (existing == null)
            {
                eventDictionary.Remove(type);
            }
            else
            {
                eventDictionary[type] = existing;
            }
        }
    }
}