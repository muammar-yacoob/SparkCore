using SparkCore.Runtime.Core.Events;

namespace SparkCoreDev.Demo.Events_Demo
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