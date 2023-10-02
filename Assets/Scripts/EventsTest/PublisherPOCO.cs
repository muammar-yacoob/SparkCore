namespace SparkDev.Demo.EventsTest
{
    public class PublisherPOCO
    {
        public PublisherPOCO()
        {
            EventManager.Instance.PublishEvent(new CustomEvent("Hello Events from POCO!"));
        }
    }
}