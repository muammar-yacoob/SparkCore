using SparkCore.Runtime.Core;
using SparkCore.Runtime.Injection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SparkDev.Demo.InjectionTest
{
    public class InjectionTest : InjectableMonoBehaviour
    {
        //Injects the first registered service of type ILogger
        [Inject] private ILogger _logger ;
        [Inject] private ILogger logger { get; set; }


        // [Inject(typeof(ConsoleLogger))] private ILogger logger; //Registered as Singleton, will inject the same instance of ConsoleLogger
        // [Inject(typeof(AnotherLogger))] private ILogger logger; //Registered as Transient, will inject a new instance of AnotherLogger

        protected override void Awake()
        {
            base.Awake();
            Debug.Log($"Press space bar to switch scenes.");
        }

        [Inject]//Called first upon service registration
        private void SetupLogger(ILogger logger)
        {
            logger.Log($"Hello Method Injection!");
        }

        private void OnEnable()
        {
            _logger.Log($"Hello field Injection!");
            logger.Log($"Hello property Injection!");
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