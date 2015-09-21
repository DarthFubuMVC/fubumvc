using System;
using FubuMVC.Core.ServiceBus;

namespace DiagnosticsHarness
{
    
    public class HarnessSettings
    {
        public HarnessSettings()
        {
            Console.WriteLine("Just being different again");

            //Channel = "memory://harness".ToUri();
            //Publisher = "memory://publisher".ToUri();

            //Use this instead if you want to test with LightningQueues
            Channel = "lq.tcp://localhost:9998/channel".ToUri();
            Publisher = "lq.tcp://localhost:9999/publisher".ToUri();

            SampleJobTime = 5000;
        }

        public Uri Publisher { get; set; }

        public Uri Channel { get; set; }

        public double SampleJobTime { get; set; }
    }
}