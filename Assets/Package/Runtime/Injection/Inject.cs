using VContainer;

namespace SparkCore.Runtime.Injection
{
    using System;

    /// <summary>
    /// Attribute to mark a field or property to be injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class Inject : InjectAttribute
    {
        /// <summary>
        /// The type of the implementation to be injected.
        /// </summary>
        public Type ImplementationType { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="Inject"/> attribute.
        ///<param name="implementationType">The type of the implementation to be injected.</param>
        /// </summary>
        /// <remarks>
        /// If the <paramref name="implementationType"/> is not specified, the type of the field or property will be used.
        /// </remarks>
        public Inject(Type implementationType = null)
        {
            ImplementationType = implementationType;
        }
    }

}