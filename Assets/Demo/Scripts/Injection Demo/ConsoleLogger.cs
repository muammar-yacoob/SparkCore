using SparkCore.Runtime.Injection;
using UnityEngine;

namespace SparkCoreDev.Demo.Injection_Demo
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