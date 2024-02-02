using System;

namespace SparkCore.Runtime.Core
{
    public class MonoEvent
    {
        private DateTime _callTime;
        public DateTime CallTime => _callTime;
        public MonoEvent()
        {
            _callTime = DateTime.Now;
        }
    }
}