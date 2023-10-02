using System;
using System.Collections.Generic;
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
        
        private readonly List<Action> unsubscribeActions = new List<Action>();
        protected void PublishEvent<T>(T eventType)
        {
            EventManager.Instance.PublishEvent(eventType);
        }

        protected void SubscribeEvent<T>(Action<T> action)
        {
            EventManager.Instance.SubscribeEvent(action);
            unsubscribeActions.Add(() => EventManager.Instance.UnsubscribeEvent(action));
        }
        
        protected void UnsubscribeEvent<T>(Action<T> action)
        {
            EventManager.Instance.UnsubscribeEvent(action);
            unsubscribeActions.Remove(() => EventManager.Instance.UnsubscribeEvent(action));
        }

        private void OnDestroy()
        {
            foreach (var unsubscribeAction in unsubscribeActions)
            {
                unsubscribeAction?.Invoke();
            }

            unsubscribeActions.Clear();
        }
    }
}