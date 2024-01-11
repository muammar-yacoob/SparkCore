using System.Diagnostics;
using SparkCore.Runtime.Injection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SparkCore.Runtime.Utils
{
    /// <summary>
    /// Startup class for editor. Loads debugger and runtime injector from Resources folder.
    /// </summary>
    public static class Bootstrapper
    {
        private const string _debugger = "Debugger";
        private const string _runtimeInjector = "Runtime Injector";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Startup()
        {
            Debug.Log($"Starting up from {nameof(Bootstrapper)}");
            
            var Debugger = Object.FindObjectsOfType(typeof(Debugger));
            if (Debugger.Length == 0) LoadResource(Bootstrapper._debugger);

            var RuntimeInjector = Object.FindObjectsOfType(typeof(RuntimeInjector));
            if (RuntimeInjector.Length == 0) LoadResource(Bootstrapper._runtimeInjector);
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
    }
}