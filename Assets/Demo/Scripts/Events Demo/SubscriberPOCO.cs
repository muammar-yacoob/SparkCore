using SparkCore.Runtime.Core;
using SparkCore.Runtime.Core.Events;

namespace SparkCoreDev.Demo.Events_Demo
{
    public class SubscriberPOCO
    {
        public SubscriberPOCO()
        {
            EventManager.Instance.SubscribeEvent<CustomEvent>(HandleCustomEvent);
        }

        private void HandleCustomEvent(CustomEvent customEvent)
        {
            UnityEngine.Debug.Log($"[POCO] Received event: {customEvent.Message}");
        }
    }
}