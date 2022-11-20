using UnityEngine;

namespace SparkCore.Runtime.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if(Instance != null) Destroy(gameObject);
            Instance = this as T;
        }
    }
}