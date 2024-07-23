using System;

namespace SparkCore.Runtime.Injection
{
    /// <summary>
    /// Used to determine how the object should be instantiated.
    /// </summary>
    public enum ServiceLifetime
    {
        Singleton,
        Transient
    }

    /// <summary>
    /// Used to mark a class as a runtime object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceProvider : Attribute
    {
        /// <summary>
        /// Determines how the object should be instantiated.
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; }
        
        /// <summary>
        /// Determines the order of the type registration in the runtime RuntimeInjector.
        /// </summary>
        public int? Order { get; } 
        
        /// <summary>
        /// Default order of the type registration in the runtime RuntimeInjector.
        /// </summary>
        public const int DefaultOrder = int.MaxValue;

        /// <summary>
        /// Creates a new instance of the <see cref="ServiceProvider"/> attribute.
        /// </summary>
        /// <param name="serviceLifetime">Lifetime of the service.</param>
        /// <param name="order">Order of the type registration in the <see cref="RuntimeInjector"/>.Services with lower numbers are registered first, followed by higher numbers, and finally unnumbered services.</param>
        public ServiceProvider(ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, int order = DefaultOrder)
        {
            ServiceLifetime = serviceLifetime;
            Order = order;
        }
    }
}