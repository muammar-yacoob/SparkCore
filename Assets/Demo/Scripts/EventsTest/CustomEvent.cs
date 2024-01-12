using SparkDev.Demo.EventsTest;

namespace SparkCoreDev.Demo.EventsTest
{
    public class CustomEvent : SceneEvent
    {
        public readonly string Message;

        public CustomEvent(string message)
        {
            Message = message;
        }
    }
}