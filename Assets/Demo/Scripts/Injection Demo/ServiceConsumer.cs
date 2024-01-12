using SparkCore.Runtime.Core.Injection;
using SparkCore.Runtime.Injection;

namespace SparkCoreDev.Demo.Injection_Demo
{
    public class ServiceConsumer : InjectableMonoBehaviour
    {
        [Inject(typeof(ConsoleLogger))] private ILogger consoleLogger;
        [Inject(typeof(AnotherLogger))] private ILogger anotherLogger;
        
        [Inject(typeof(AnotherLogger))] public ILogger secondLogger { get; set; }

        private void OnEnable()
        {
            anotherLogger.Log("Hello field Injection!");
            secondLogger.Log("Hello property Injection!");
        }
    }
}