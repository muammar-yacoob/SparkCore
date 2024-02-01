using SparkCore.Runtime.Core;

namespace SparkDev.Demo.EventsTest
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