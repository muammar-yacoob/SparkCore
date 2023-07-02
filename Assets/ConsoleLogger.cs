using SparkCore.Runtime;
using UnityEngine;
using VContainer;

namespace SparkCoreDev
{
    [Injectable(Lifetime.Singleton)]
    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Debug.Log($"ConsoleLogger: {message}");
    }
    
    [Injectable(Lifetime.Singleton)]
    public class AnotherLogger : ILogger
    {
        public void Log(string message) => Debug.Log($"AnotherLogger: {message}");
    }

}