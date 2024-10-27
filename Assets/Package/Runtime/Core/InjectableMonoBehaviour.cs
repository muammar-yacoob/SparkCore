using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SparkCore.Runtime.Injection;
using UnityEngine;

namespace SparkCore.Runtime.Core
{
    /// <summary>
    /// Base class for all MonoBehaviours that want to use Dependency Injection and Event Subscriptions.
    /// </summary>
    public abstract class InjectableMonoBehaviour : MonoBehaviour
    {
        #region Injection

        protected virtual void Awake()
        {
            InjectDependencies(this);
        }

        private void InjectDependencies(object injectableMonoBehaviour)
        {
            var container = RuntimeInjector.Container;
            InjectFields(injectableMonoBehaviour, container);
            InjectProperties(injectableMonoBehaviour, container);
            InjectMethods(injectableMonoBehaviour, container);
        }

        private void InjectFields(object injectableMonoBehaviour, Container container)
        {
            var fields = injectableMonoBehaviour.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.IsDefined(typeof(Inject), true));

            foreach (var field in fields)
            {
                var injectAttribute = field.GetCustomAttribute<Inject>();
                var typeToInject = injectAttribute?.ImplementationType ?? field.FieldType;
                var resolvedInstance = container.Resolve(typeToInject, null, this);
                field.SetValue(injectableMonoBehaviour, resolvedInstance);
            }
        }

        private void InjectProperties(object injectableMonoBehaviour, Container container)
        {
            var properties = injectableMonoBehaviour.GetType()
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(Inject), true));

            foreach (var property in properties)
            {
                var injectAttribute = property.GetCustomAttribute<Inject>();
                var typeToInject = injectAttribute?.ImplementationType ?? property.PropertyType;
                var resolvedInstance = container.Resolve(typeToInject, null, this);
                property.SetValue(injectableMonoBehaviour, resolvedInstance);
            }
        }

        private void InjectMethods(object injectableMonoBehaviour, Container container)
        {
            var methods = injectableMonoBehaviour.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.IsDefined(typeof(Inject), true));

            foreach (var method in methods)
            {
                var injectAttribute = method.GetCustomAttribute<Inject>();
                var typeToInject = injectAttribute?.ImplementationType ?? method.GetParameters().First().ParameterType;
                var resolvedInstance = container.Resolve(typeToInject, null, this);
                method.Invoke(injectableMonoBehaviour, new[] { resolvedInstance });
            }
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
            if (delegateToUnsubscribeAction.TryGetValue(action, out var unsubscribeAction))
            {
                unsubscribeAction.Invoke();
                delegateToUnsubscribeAction.Remove(action);
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (var unsubscribeAction in delegateToUnsubscribeAction.Values)
            {
                unsubscribeAction?.Invoke();
            }

            delegateToUnsubscribeAction.Clear();
        }

        #endregion
    }
}