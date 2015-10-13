using System;
using FubuMVC.Core;

namespace DiagnosticsHarness
{
    public class TraceMessage
    {
        public TraceMessage()
        {
            Name = Guid.NewGuid().ToString();
        }

        public string Name { get; set; } 
    }

    
    public class TraceHandler
    {
        [WrapWith(typeof(DelayWrapper))]
        public NumberMessage Consume(TraceMessage message)
        {
            return new NumberMessage
            {
                Value = 100
            };
        }
    }
}