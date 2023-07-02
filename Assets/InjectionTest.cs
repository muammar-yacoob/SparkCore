using SparkCore.Runtime;
using VContainer;

namespace SparkCoreDev
{
    public class InjectionTest : InjectableMonoBehaviour
    {
        [Inject] private ConsoleLogger logger;

        private void OnEnable()
        {
            logger.Log("Hello World!");
        }
    }
}
    