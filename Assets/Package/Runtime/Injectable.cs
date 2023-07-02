using System;
using VContainer;

namespace SparkCore.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Injectable : Attribute
    {
        public Lifetime Lifetime { get; }

        public Injectable(Lifetime lifetime = Lifetime.Singleton)
        {
            Lifetime = lifetime;
        }
    }
}