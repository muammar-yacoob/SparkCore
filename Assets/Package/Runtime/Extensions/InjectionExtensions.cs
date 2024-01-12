using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SparkCore.Runtime.Utils
{
    public class InjectionExtensions
    {
        //Helpers
        public static Assembly GetDefualtAssembly() => GetAssemblyByName("Assembly-CSharp");

        static Assembly GetAssemblyByName(string name) => AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == name);

        static IEnumerable<Type> GetTypesInDefaultAssembly<T>() => GetDefualtAssembly().GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
    }
}