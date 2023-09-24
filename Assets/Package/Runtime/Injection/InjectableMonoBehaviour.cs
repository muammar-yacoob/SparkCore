using UnityEngine;

namespace SparkCore.Runtime.Injection
{
    public class InjectableMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            var container = RuntimeInjector.Instance.Container;
            container.Inject(this);
        }
    }
}