using SparkCore.Runtime.Core;
using SparkCore.Runtime.Injection;

namespace SparkCoreDev.Demo.InjectionTest
{
    public class InjectionTest : InjectableMonoBehaviour
    {
        [Inject(typeof(ConsoleLogger))] private ILogger logger;

        private void OnEnable()
        {
            logger.Log("Hello Injection!");
        }
    }
}