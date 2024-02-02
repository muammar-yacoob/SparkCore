using SparkCore.Runtime.Core;

namespace SparkCoreDev.Demo.Scripts.Events
{
    public class PublisherMono : InjectableMonoBehaviour
    {
        private void OnEnable()
        {
            var customEvent = new CustomEvent("Hello Mono Events from PublisherMono!");
            PublishEvent(customEvent);
            
            // SubscriberPOCO subscriberPoco = new SubscriberPOCO();
            // PublisherPOCO publisherPoco = new PublisherPOCO();
        }
    }
}