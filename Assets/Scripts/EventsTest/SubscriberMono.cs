using SparkCore.Runtime.Core;
using UnityEngine;

namespace SparkDev.Demo.EventsTest
{
    public class SubscriberMono : InjectableMonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            SubscribeEvent<CustomEvent>(HandleCustomEvent);
        }

        //private void OnDestroy() => UnsubscribeEvent<CustomEvent>(HandleEvent);

        void HandleCustomEvent(CustomEvent customEvent)
        {
            Debug.Log($"Received event: {customEvent.Message}");
        }
    }
}