using System;

namespace SparkDev.Demo.EventsTest
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