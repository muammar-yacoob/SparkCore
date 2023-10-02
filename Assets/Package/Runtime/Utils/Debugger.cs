using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SparkCore.Runtime.Utils
{
    public class Debugger : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log($"{nameof(Debugger)} instantiated", this);
            if (SceneManager.GetActiveScene().buildIndex != 0) SceneManager.LoadScene(0);
            //ClearConsole();
#endif
        }

        private static void ClearConsole()
        {
#if UNITY_EDITOR
            var logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear",
                BindingFlags.Static | BindingFlags.Public);
            clearMethod.Invoke(null, null);
#endif
        }
    }
}