using SparkCore.Runtime.Core;
using SparkCore.Runtime.Injection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SparkCoreDev.Demo.Scripts.Injection
{
    public class InjectionTest : InjectableMonoBehaviour
    {
        //Injects the first registered service of type ILogger
        [Inject] private ILogger loggerField ;
        [Inject(typeof(AnotherLogger))] private ILogger loggerField2 ;
        [Inject] private ILogger loggerProp { get; set; }
        [Inject] private ILogger loggerProp2 { get; set; }


        // [Inject(typeof(ConsoleLogger))] private ILogger logger; //Registered as Singleton, will inject the same instance of ConsoleLogger
        // [Inject(typeof(AnotherLogger))] private ILogger logger; //Registered as Transient, will inject a new instance of AnotherLogger

        protected override void Awake()
        {
            base.Awake();
            Debug.Log($"Press space bar to switch scenes.");
        }

        // [Inject]
        [Inject(typeof(AnotherLogger))]
        private void SetupLogger(ILogger loggerParam)
        {
            loggerParam.Log($"Hello Method Injection!");
        }

        private void OnEnable()
        {
            loggerField.Log($"Hello field Injection!");
            loggerProp.Log($"Hello property Injection!");
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