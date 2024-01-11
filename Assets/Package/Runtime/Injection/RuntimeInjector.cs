using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SparkCore.Runtime.Injection
{
    /// <summary>
    /// Responsible for registering all types with the <see cref="RuntimeObject"/> attribute with VContainer.
    /// </summary>
    public class RuntimeInjector : LifetimeScope
    {
        public static RuntimeInjector Instance { get; private set; }

        protected override void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);
            base.Awake();
        }


        protected override void Configure(IContainerBuilder builder)
        {
            AutoRegister(builder);
        }

        private static void AutoRegister(IContainerBuilder builder)
        {
            var scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var injectableTypes = scriptAssemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<RuntimeObject>() != null);

            // Register with VContainer
            foreach (var type in injectableTypes)
            {
                var lifetime = type.GetCustomAttribute<RuntimeObject>().RuntimeObjectType;

                builder.Register(type, MapToVContainerLifetime(lifetime))
                    .AsImplementedInterfaces()
                    .AsSelf();
                Debug.Log($"{type.Name} registered with VContainer");
            }
            Debug.Log(injectableTypes.Count() + " types registered with VContainer");
        }
        
        private static Lifetime MapToVContainerLifetime(RuntimeObjectType runtimeObjectType)
        {
            return runtimeObjectType switch
            {
                RuntimeObjectType.Singleton => Lifetime.Singleton,
                RuntimeObjectType.Transient => Lifetime.Transient,
                _ => Lifetime.Singleton,
            };
        }
    }
}