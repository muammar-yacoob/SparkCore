using System;
using System.Linq;
using System.Reflection;
using Palmmedia.ReportGenerator.Core.Logging;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SparkCore.Runtime
{
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

            //manual registration to enforce custom implementations.
            //builder.Register<ILogger,ConsoleLogger>(Lifetime.Singleton);
        }

        private static void AutoRegister(IContainerBuilder builder)
        {
            var scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name.StartsWith("Assembly-CSharp"));

            var injectableTypes = scriptAssemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<Injectable>() != null);

            // Register with VContainer
            foreach (var type in injectableTypes)
            {
                var lifetime = type.GetCustomAttribute<Injectable>().Lifetime;

                builder.Register(type, lifetime)
                    .AsImplementedInterfaces()
                    .AsSelf();
            }
            Debug.Log(injectableTypes.Count() + " types registered with VContainer");
        }

    }
}