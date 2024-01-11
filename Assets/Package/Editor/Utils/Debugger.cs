using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SparkCore.Editor.Utils
{
    public class Debugger : MonoBehaviour
    {
        [SerializeField] private bool clearConsoleOnPlay = true;
        private async void Awake()
        {
#if UNITY_EDITOR
            Debug.Log($"{nameof(Debugger)} instantiated", this);
            if (SceneManager.GetActiveScene().buildIndex != 0)
                await new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 0);

            if (clearConsoleOnPlay) ClearConsole();
#endif
        }

        private static void ClearConsole()
        {
#if UNITY_EDITOR
            var logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries?.GetMethod("Clear",
                BindingFlags.Static | BindingFlags.Public);
            clearMethod?.Invoke(null, null);
#endif
        }
    }
}