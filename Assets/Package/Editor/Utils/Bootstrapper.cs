using SparkCore.Runtime;
using UnityEngine;

namespace SparkCore.Editor.Utils
{
    public static class Bootstrapper
    {
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Startup()
        {
            Debug.Log($"Starting up from {nameof(Bootstrapper)}");

            LoadResource("Debugger");
            LoadResource("Runtime Injector");
        }

        private static void LoadResource(string prefabName)
        {
            var sceneObject = GameObject.Find(prefabName);
            if (sceneObject != null)
            {
                Debug.LogError($"Remove {prefabName} from scene. It will be loaded automatically from Resources folder.", sceneObject);
                Object.DestroyImmediate(sceneObject);
            }
            
            var prefab = Resources.Load(prefabName);
            if (prefab != null)
            {
                Object.DontDestroyOnLoad((Object.Instantiate(prefab)));
            }
        }
#endif
    }
}