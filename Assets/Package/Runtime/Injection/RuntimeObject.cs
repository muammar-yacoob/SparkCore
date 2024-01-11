using System;

namespace SparkCore.Runtime.Injection
{
    /// <summary>
    /// Used to determine how the object should be instantiated.
    /// </summary>
    public enum RuntimeObjectType
    {
        Singleton,
        Transient
    }


    /// <summary>
    /// Used to mark a class as a runtime object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RuntimeObject : Attribute
    {
        /// <summary>
        /// Determines how the object should be instantiated.
        /// </summary>
        public RuntimeObjectType RuntimeObjectType { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="RuntimeObject"/> attribute.
        /// </summary>
        /// <param name="runtimeObjectType"></param>
        public RuntimeObject(RuntimeObjectType runtimeObjectType = RuntimeObjectType.Singleton)
        {
            RuntimeObjectType = runtimeObjectType;
        }
    }

}