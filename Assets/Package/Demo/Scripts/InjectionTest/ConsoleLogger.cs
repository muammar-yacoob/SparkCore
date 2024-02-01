using SparkCore.Runtime.Injection;
using UnityEngine;

namespace SparkDev.Demo.InjectionTest
{
    [ServiceProvider(ServiceLifetime.Singleton)]
    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Debug.Log($"ConsoleLogger: {message}");
    }
    
    [ServiceProvider(ServiceLifetime.Singleton)]
    public class AnotherLogger : ILogger
    {
        public void Log(string message) => Debug.Log($"AnotherLogger: {message}");
    }

}