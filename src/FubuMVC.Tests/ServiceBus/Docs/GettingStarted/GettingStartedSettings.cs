using System;
using FubuMVC.Core.ServiceBus;

namespace FubuMVC.Tests.ServiceBus.Docs.GettingStarted
{
    // SAMPLE: GettingStartedSettings
    public class GettingStartedSettings
    {
        public GettingStartedSettings()
        {
            //Defaulting to only in memory for sample
            Uri = "memory://gettingstarted".ToUri();
        }

        public Uri Uri { get; set; }
    }
    // ENDSAMPLE
}