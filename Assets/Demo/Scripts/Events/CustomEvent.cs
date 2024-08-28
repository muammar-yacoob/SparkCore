using SparkCore.Runtime.Core;

namespace SparkCoreDev.Demo.Scripts.Events
{
    public class CustomEvent : MonoEvent
    {
        public readonly string Message;

        public CustomEvent(string message)
        {
            Message = message;
        }
    }
    
    public class GameStartedEvent : MonoEvent
    {
    }
}