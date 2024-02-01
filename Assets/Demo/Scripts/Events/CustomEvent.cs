using SparkCore.Runtime.Core;

namespace SparkCoreDev.Demo.Scripts.Events
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