using SparkCore.Runtime.Core;
using SparkCore.Runtime.Core.Events;

namespace SparkCoreDev.Demo.Events_Demo
{
    public class PublisherPOCO
    {
        public PublisherPOCO()
        {
            EventManager.Instance.PublishEvent(new CustomEvent("Hello Events from POCO!"));
        }
    }
}