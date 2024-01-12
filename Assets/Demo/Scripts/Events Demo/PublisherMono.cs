using SparkCore.Runtime.Core;
using SparkCore.Runtime.Core.Injection;

namespace SparkCoreDev.Demo.Events_Demo
{
    public class PublisherMono : InjectableMonoBehaviour
    {
        private void OnEnable()
        {
            var customEvent = new CustomEvent("Hello Events from Mono!");
            PublishEvent(customEvent);
            
            // SubscriberPOCO subscriberPoco = new SubscriberPOCO();
            // PublisherPOCO publisherPoco = new PublisherPOCO();
        }
    }
}