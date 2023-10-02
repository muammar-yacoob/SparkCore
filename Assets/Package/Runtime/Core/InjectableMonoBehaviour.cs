using System;
using SparkCore.Runtime.Injection;
using UnityEngine;

namespace SparkCore.Runtime.Core
{
    public abstract class InjectableMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            var container = RuntimeInjector.Instance.Container;
            container.Inject(this);
        }
    
        protected void PublishEvent<T>(T eventType)
        {
            EventManager.Instance.PublishEvent(eventType);
        }

        protected void SubscribeEvent<T>(Action<T> action)
        {
            EventManager.Instance.SubscribeEvent(action);
        }
        
        protected void UnsubscribeEvent<T>(Action<T> action)
        {
            EventManager.Instance.UnsubscribeEvent(action);
        }
    }
}