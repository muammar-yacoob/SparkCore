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
            var debugger = Resources.Load("Debugger");
            if(debugger == null) return;
            Object.DontDestroyOnLoad((Object.Instantiate(debugger)));
        }
#endif
    }
}