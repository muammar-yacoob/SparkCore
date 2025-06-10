using System;

namespace SparkCore.Runtime.Core
{
    public sealed class SparkCoreRuntime
    {
        public static SparkCoreRuntime Instance { get; private set; }

        private SparkCoreRuntime()
        {
            // Private constructor to enforce singleton pattern
        }

        // App Lifetime events
        public static event Action AppLostFocus;
        public static event Action AppGainedFocus;
        public static event Action AppPaused;
        public static event Action AppResumed;
        public static event Action AppEnding;
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) AppGainedFocus?.Invoke();
            else AppLostFocus?.Invoke();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) AppPaused?.Invoke();
            else AppResumed?.Invoke();
        }

        private void OnApplicationQuit() => AppEnding?.Invoke();
    }
}