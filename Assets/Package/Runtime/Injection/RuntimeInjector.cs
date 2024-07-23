using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SparkCore.Runtime.Utils;
using UnityEngine;
using SparkCore.Runtime.Core;

namespace SparkCore.Runtime.Injection
{
    /// <summary>
    /// Responsible for registering all types with the <see cref="ServiceProvider"/> attribute.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class RuntimeInjector : Singleton<RuntimeInjector>
    {
        private static readonly Lazy<Container> _container = new(BuildContainer);
        public static Container Container => _container.Value;

        private static Container BuildContainer()
        {
            var services = new List<ServiceDescriptor>();
            var scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Find types with ServiceProvider attribute
            var injectableTypes = scriptAssemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<ServiceProvider>() != null)
                .OrderBy(t => t.GetCustomAttribute<ServiceProvider>().Order)
                .ToList();

            foreach (var type in injectableTypes)
            {
                var serviceProviderAttr = type.GetCustomAttribute<ServiceProvider>();
                var lifetime = serviceProviderAttr.ServiceLifetime;
                var order = serviceProviderAttr.Order;

                // Register the concrete type itself
                services.Add(new ServiceDescriptor(type, type, lifetime, order));
                var debugMessage = $"{type.Name} registered as {lifetime}";
                var orderMessage = order == Int32.MaxValue ? $" with default order." : $" with order {order}.";
                Debug.Log(debugMessage + orderMessage);

                // Register interfaces implemented by the type
                foreach (var serviceType in type.GetInterfaces())
                {
                    services.Add(new ServiceDescriptor(serviceType, type, lifetime, order));
                }
            }

            Debug.Log($"{services.Count} services registered.");
            return new Container(services);
        }
    }

    public class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public Type ImplementationType { get; private set; }
        public object Implementation { get; internal set; }
        public ServiceLifetime Lifetime { get; }
        public int? Order { get; }

        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime,
            int? order, object implementation = null)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            ImplementationType =
                implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            Lifetime = lifetime;
            Order = order;
            Implementation = implementation;
        }
    }

    public class Container
    {
        private readonly List<ServiceDescriptor> serviceDescriptors;

        public Container(List<ServiceDescriptor> serviceDescriptors)
        {
            this.serviceDescriptors =
                serviceDescriptors ?? throw new ArgumentNullException(nameof(serviceDescriptors));
        }

        public void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime, int? order = null)
        {
            serviceDescriptors.Add(new ServiceDescriptor(serviceType, implementationType, lifetime, order));
        }

        public void Register<TService, TImplementation>(ServiceLifetime lifetime)
            where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation), lifetime);
        }

        public object Resolve(Type serviceType, Type implementationType = null,
            InjectableMonoBehaviour context = null)
        {
            ServiceDescriptor descriptor;

            if (implementationType != null)
            {
                descriptor = serviceDescriptors
                    .Where(x => x.ImplementationType == implementationType)
                    .OrderBy(x => x.Order)
                    .FirstOrDefault();
            }
            else
            {
                descriptor = serviceDescriptors
                    .Where(x => x.ServiceType == serviceType || x.ImplementationType == serviceType)
                    .OrderBy(x => x.Order)
                    .FirstOrDefault();
            }

            if (descriptor == null)
            {
                if (!serviceType.IsAbstract && !serviceType.IsInterface)
                    return CreateInstance(serviceType, context);

                throw new InvalidOperationException(
                    $"Service of type {serviceType.Name} is not registered. Make sure it is marked with the ServiceProvider attribute");
            }

            if (descriptor.Lifetime == ServiceLifetime.Singleton && descriptor.Implementation != null)
                return descriptor.Implementation;

            var typeToCreate = implementationType ?? descriptor.ImplementationType ?? serviceType;
            var implementation = CreateInstance(typeToCreate, context);

            if (descriptor.Lifetime == ServiceLifetime.Singleton)
                descriptor.Implementation = implementation;

            return implementation;
        }

        public T Resolve<T>(Type implementationType = null, InjectableMonoBehaviour context = null)
        {
            return (T)Resolve(typeof(T), implementationType, context);
        }

        private object CreateInstance(Type type, InjectableMonoBehaviour context = null)
        {
            string contextName = context != null ? context.GetType().Name : "Unknown";

            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                Debug.LogWarning(
                    $"[{contextName}] Attempted to create MonoBehaviour {type.Name} using the container. Searching for existing instance in scene.",
                    context);
                var existingInstance = UnityEngine.Object.FindObjectOfType(type) as MonoBehaviour;
                if (existingInstance != null)
                {
                    Debug.LogWarning(
                        $"[{contextName}] Found existing instance of {type.Name} in scene. Using this instance, but consider refactoring to avoid this.",
                        context);
                    return existingInstance;
                }
                else if (context != null)
                {
                    Debug.LogWarning(
                        $"[{contextName}] No existing instance of MonoBehaviour {type.Name} found in scene. Adding it as a component to the requesting object.",
                        context);
                    return context.gameObject.AddComponent(type);
                }
                else
                {
                    Debug.LogError(
                        $"[{contextName}] No existing instance of MonoBehaviour {type.Name} found in scene, and no context provided to add it as a component. MonoBehaviours should be added using AddComponent().",
                        context);
                    throw new InvalidOperationException(
                        $"Cannot create MonoBehaviour {type.Name} using the container. Use AddComponent() instead.");
                }
            }

            var constructors =
                type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (constructors.Length == 0)
                throw new InvalidOperationException($"[{contextName}] No constructor found for {type.Name}.");

            // Sort constructors by parameter count (descending)
            var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();

            var parameters = new List<object>();
            foreach (var param in constructor.GetParameters())
            {
                try
                {
                    var resolvedParam = Resolve(param.ParameterType, null, context);
                    parameters.Add(resolvedParam);
                }
                catch (InvalidOperationException ex)
                {
                    if (param.HasDefaultValue)
                        parameters.Add(param.DefaultValue);
                    else
                        throw new InvalidOperationException(
                            $"[{contextName}] Failed to resolve parameter {param.Name} of type {param.ParameterType.Name} for {type.Name}",
                            ex);
                }
            }

            try
            {
                return constructor.Invoke(parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"[{contextName}] Failed to create instance of {type.Name}. Constructor parameters: {string.Join(", ", parameters.Select(p => p?.GetType().Name ?? "null"))}",
                    ex);
            }
        }
    }
}