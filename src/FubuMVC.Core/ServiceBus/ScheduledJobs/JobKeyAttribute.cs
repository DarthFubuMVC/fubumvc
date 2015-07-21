using System;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JobKeyAttribute : Attribute
    {
        private readonly string _key;

        public JobKeyAttribute(string key)
        {
            _key = key;
        }

        public string Key
        {
            get { return _key; }
        }
    }
}