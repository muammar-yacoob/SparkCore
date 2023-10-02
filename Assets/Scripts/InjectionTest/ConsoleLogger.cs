using SparkCore.Runtime.Injection;
using UnityEngine;

namespace SparkDev.Demo.InjectionTest
{
    [RuntimeObject(RuntimeObjectType.Singleton)]
    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Debug.Log($"ConsoleLogger: {message}");
    }
    
    [RuntimeObject(RuntimeObjectType.Singleton)]
    public class AnotherLogger : ILogger
    {
        public void Log(string message) => Debug.Log($"AnotherLogger: {message}");
    }

}