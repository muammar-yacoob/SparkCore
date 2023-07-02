using SparkCore.Runtime;
using VContainer;

namespace SparkCoreDev
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
    