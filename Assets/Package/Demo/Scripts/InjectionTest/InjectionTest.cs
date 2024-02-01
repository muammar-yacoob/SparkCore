using System;
using SparkCore.Runtime.Core;
using SparkCore.Runtime.Injection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SparkDev.Demo.InjectionTest
{
    public class InjectionTest : InjectableMonoBehaviour
    {
        [Inject] private ILogger logger; // will inject the first registered service of type ILogger
        
        // [Inject(typeof(ConsoleLogger))] private ILogger logger;
        //[Inject(typeof(AnotherLogger))] private ILogger logger;

        private void OnEnable()
        {
            logger.Log($"Hello Injection!");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                
                int nextSceneIndex = currentSceneIndex == 0 ? 1 : 0;
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }
}