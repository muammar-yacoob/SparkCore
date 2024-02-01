using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SparkCore.Runtime.Utils;
using UnityEngine;

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
                .ToList();

            foreach (var type in injectableTypes)
            {
                var lifetime = type.GetCustomAttribute<ServiceProvider>()?.ServiceLifetime ?? ServiceLifetime.Transient;

                // Find interfaces implemented by the type
                var serviceTypes = type.GetInterfaces();
                foreach (var serviceType in serviceTypes)
                {
                    object implementationInstance = null;
                    if (lifetime == ServiceLifetime.Singleton)
                    {
                        implementationInstance = Activator.CreateInstance(type);
                    }

                    services.Add(new ServiceDescriptor(serviceType, type, lifetime, implementationInstance));
                    Debug.Log($"{serviceType.Name} -> {type.Name} registered as {lifetime}");
                }
            }

            Debug.Log($"{injectableTypes.Count} types registered");
            return new Container(services);
        }
    }

    public class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public Type ImplementationType { get; private set; }
        public object Implementation { get; internal set; }
        public ServiceLifetime Lifetime { get; }

        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime,
            object implementation = null)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            Lifetime = lifetime;
            Implementation = implementation;
        }
    }


    public class Container
    {
        private readonly List<ServiceDescriptor> _serviceDescriptors;

        public Container(List<ServiceDescriptor> serviceDescriptors)
        {
            _serviceDescriptors = serviceDescriptors ?? throw new ArgumentNullException(nameof(serviceDescriptors));
        }

        public object Resolve(Type serviceType, Type implementationType = null)
        {
            ServiceDescriptor descriptor;
            if (implementationType != null)
            {
                descriptor = _serviceDescriptors
                    .FirstOrDefault(x => x.ImplementationType == implementationType);
                if (descriptor == null)
                {
                    throw new InvalidOperationException(
                        $"Concrete implementation of type {implementationType.Name} is not registered.");
                }
            }
            else
            {
                descriptor = _serviceDescriptors
                    .FirstOrDefault(x => x.ServiceType == serviceType);
                if (descriptor == null)
                {
                    // Fallback for concrete types not explicitly registered
                    if (!serviceType.IsAbstract && !serviceType.IsInterface)
                        return CreateInstance(serviceType);

                    throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered.");
                }
            }

            if (descriptor.Lifetime == ServiceLifetime.Singleton && descriptor.Implementation != null)
                return descriptor.Implementation;

            var typeToCreate = implementationType ?? descriptor.ImplementationType ?? serviceType;
            var implementation = CreateInstance(typeToCreate);

            if (descriptor.Lifetime == ServiceLifetime.Singleton)
                descriptor.Implementation = implementation;

            return implementation;
        }

        public T Resolve<T>(Type implementationType = null)
        {
            return (T)Resolve(typeof(T), implementationType);
        }

        private object CreateInstance(Type type)
        {
            var constructorInfo = type.GetConstructors().FirstOrDefault();
            if (constructorInfo == null)
                throw new InvalidOperationException($"No public constructor found for {type.Name}.");

            var parameters = constructorInfo.GetParameters()
                .Select(x => Resolve(x.ParameterType)).ToArray();

            return Activator.CreateInstance(type, parameters);
        }
    }
}