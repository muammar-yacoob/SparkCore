using SparkCore.Runtime.Core;
using VContainer;

namespace SparkDev.Demo.InjectionTest
{
    public class InjectionTest : InjectableMonoBehaviour
    {
        [Inject] private ILogger logger;

        private void OnEnable()
        {
            logger.Log("Hello World!");
        }
    }
}
    