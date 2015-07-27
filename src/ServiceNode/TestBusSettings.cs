using System;
using FubuMVC.Core.ServiceBus;

namespace ServiceNode
{
    public class TestBusSettings
    {
        public TestBusSettings()
        {
            Service = "lq.tcp://localhost:2215/service".ToUri();
            Website = "lq.tcp://localhost:2216/website".ToUri();
        }

        public Uri Service { get; set; }
        public Uri Website { get; set; }
    }
}