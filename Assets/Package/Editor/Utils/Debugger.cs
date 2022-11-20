using UnityEngine;
using UnityEngine.SceneManagement;

namespace SparkCore.Editor.Utils
{
    public class Debugger : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Awake()
        {
            SceneManager.LoadScene(0);
            ClearConsole();
            Debug.Log($"{nameof(Debugger)} instantiated", this);
        }
#endif
        
#if UNITY_EDITOR
        private static void ClearConsole()
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
#endif
    }
}