using SparkCore.Runtime.Core;
using SparkCore.Runtime.Core.Injection;
using UnityEngine;

namespace SparkCoreDev.Demo.Events_Demo
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