using System;

namespace SparkCore.Runtime.Core.Events
{
    public class SceneEvent
    {
        private DateTime _callTime;
        public DateTime CallTime => _callTime;
        public SceneEvent()
        {
            _callTime = DateTime.Now;
        }
    }
}