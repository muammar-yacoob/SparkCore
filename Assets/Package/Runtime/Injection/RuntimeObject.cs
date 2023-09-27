using System;

namespace SparkCore.Runtime.Injection
{
    public enum RuntimeObjectType
    {
        Singleton,
        Transient
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class RuntimeObject : Attribute
    {
        public RuntimeObjectType RuntimeObjectType { get; }

        public RuntimeObject(RuntimeObjectType runtimeObjectType = RuntimeObjectType.Singleton)
        {
            RuntimeObjectType = runtimeObjectType;
        }
    }

}