using SparkCore.Runtime.Core;

namespace SparkCoreDev.Demo.Scripts.Events
{
    public class PublisherPOCO
    {
        public PublisherPOCO()
        {
            EventManager.Instance.PublishEvent(new CustomEvent("Hello Events from POCO!"));
        }
    }
}