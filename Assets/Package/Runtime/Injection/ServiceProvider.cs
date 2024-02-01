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
        /// Creates a new instance of the <see cref="ServiceProvider"/> attribute.
        /// </summary>
        /// <param name="serviceLifetime"></param>
        public ServiceProvider(ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            ServiceLifetime = serviceLifetime;
        }
    }
}