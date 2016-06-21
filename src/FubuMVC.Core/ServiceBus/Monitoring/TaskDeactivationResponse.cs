using System;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TaskDeactivationResponse
    {
        public Uri Subject { get; set; }
        public bool Success { get; set; }
    }
}