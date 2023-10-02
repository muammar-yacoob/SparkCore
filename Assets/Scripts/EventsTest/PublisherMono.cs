using SparkCore.Runtime.Core;

namespace SparkDev.Demo.EventsTest
{
    public class PublisherMono : InjectableMonoBehaviour
    {
        private void OnEnable()
        {
            CustomEvent customEvent = new CustomEvent("Hello Events!");
            PublishEvent(customEvent);
        }
    }
}