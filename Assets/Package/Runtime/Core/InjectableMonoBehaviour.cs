using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SparkCore.Runtime.Injection;
using UnityEngine;
using VContainer;

namespace SparkCore.Runtime.Core
{
    /// <summary>
    ///   Base class for all MonoBehaviours that want to use Dependency Injection and Event Subscriptions.
    /// </summary>
    public abstract class InjectableMonoBehaviour : MonoBehaviour
    {
        #region Injection
        protected virtual void Awake() => InjectDependencies(this);

        private void InjectDependencies(object injectableMonoBehaviour)
        {
            var container = RuntimeInjector.Instance.Container;
            
            InjectFields(injectableMonoBehaviour, container);
            InjectProperties(injectableMonoBehaviour, container);
        }

        private static void InjectProperties(object injectableMonoBehaviour, IObjectResolver container)
        {
            var properties = injectableMonoBehaviour.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.IsDefined(typeof(Inject), true));

            foreach (var property in properties)
            {
                var injectAttribute = property.GetCustomAttribute<Inject>(true);
                var typeToInject = injectAttribute?.ImplementationType ?? property.PropertyType;
                
                var resolvedInstance = container.Resolve(typeToInject);
                property.SetValue(injectableMonoBehaviour, resolvedInstance);
            }
        }

        private static void InjectFields(object injectableMonoBehaviour, IObjectResolver container)
        {
            var fields = injectableMonoBehaviour.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.IsDefined(typeof(Inject), true));

            foreach (var field in fields)
            {
                var injectAttribute = field.GetCustomAttribute<Inject>(true);
                var typeToInject = injectAttribute?.ImplementationType ?? field.FieldType;

                var resolvedInstance = container.Resolve(typeToInject);
                field.SetValue(injectableMonoBehaviour, resolvedInstance);
            }
        }

        #endregion

        #region Event Subscriptions
        private readonly Dictionary<Delegate, Action> delegateToUnsubscribeAction = new ();

        /// <summary>
        /// Publishes an event to all subscribers.
        /// </summary>
        /// <param name="eventType">Event to publish</param>
        /// <typeparam name="T">Event type</typeparam>
        protected void PublishEvent<T>(T eventType)
        {
            EventManager.Instance.PublishEvent(eventType);
        }
        
        /// <summary>
        /// Subscribes to an event.
        /// </summary>
        /// <param name="action">Action to execute when the event is published.</param>
        /// <typeparam name="T">Event type</typeparam>
        protected void SubscribeEvent<T>(Action<T> action)
        {
            EventManager.Instance.SubscribeEvent(action);
            delegateToUnsubscribeAction[action] = () => EventManager.Instance.UnsubscribeEvent(action);
        }

        /// <summary>
        /// Unsubscribes from an event.
        /// </summary>
        /// <param name="action">Action to unsubscribe.</param>
        /// <typeparam name="T">Event type</typeparam>
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