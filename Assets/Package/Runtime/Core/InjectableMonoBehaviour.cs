using System;
using System.Collections.Generic;
using SparkCore.Runtime.Injection;
using UnityEngine;

namespace SparkCore.Runtime.Core
{
    public abstract class InjectableMonoBehaviour : MonoBehaviour
    {
        #region Injection

        protected virtual void Awake()
        {
            var container = RuntimeInjector.Instance.Container;
            container.Inject(this);
        }

        #endregion

        #region Event Subscriptions
        private readonly Dictionary<Delegate, Action> delegateToUnsubscribeAction = new Dictionary<Delegate, Action>();

        protected void PublishEvent<T>(T eventType)
        {
            EventManager.Instance.PublishEvent(eventType);
        }
        
        protected void SubscribeEvent<T>(Action<T> action)
        {
            EventManager.Instance.SubscribeEvent(action);
            delegateToUnsubscribeAction[action] = () => EventManager.Instance.UnsubscribeEvent(action);
        }

        protected void UnsubscribeEvent<T>(Action<T> action)
        {
            EventManager.Instance.UnsubscribeEvent(action);
            delegateToUnsubscribeAction.Remove(action);
        }

        private void OnDestroy()
        {
            foreach (var unsubscribeAction in delegateToUnsubscribeAction.Values)
            {
                unsubscribeAction?.Invoke();
            }

            delegateToUnsubscribeAction.Clear();
        }
    }
    #endregion
}