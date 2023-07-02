using SparkCore.Runtime;
using UnityEngine;

namespace SparkCore.Runtime
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