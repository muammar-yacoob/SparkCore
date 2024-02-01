using SparkCore.Runtime.Core;
using UnityEngine;

namespace SparkCoreDev.Demo.Scripts.Events
{
    public class SubscriberMono : InjectableMonoBehaviour
    {
        private void OnEnable()
        {
            SubscribeEvent<CustomEvent>(HandleCustomEvent);
        }

        private void OnDisable()
        {
            UnsubscribeEvent<CustomEvent>(HandleCustomEvent);
        }

        void HandleCustomEvent(CustomEvent customEvent)
        {
            Debug.Log($"[Mono] Received event: {customEvent.Message}");
        }
    }
}