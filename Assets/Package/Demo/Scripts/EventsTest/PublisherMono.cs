using SparkCore.Runtime.Core;

namespace SparkDev.Demo.EventsTest
{
    public class PublisherMono : InjectableMonoBehaviour
    {
        private void OnEnable()
        {
            CustomEvent customEvent = new CustomEvent("Hello Events from Mono!");
            PublishEvent(customEvent);
            
            // SubscriberPOCO subscriberPoco = new SubscriberPOCO();
            // PublisherPOCO publisherPoco = new PublisherPOCO();
        }
    }
}