using SparkCore.Runtime.Core;

namespace SparkCoreDev.Demo.EventsTest
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