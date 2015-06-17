using System;

namespace FubuTransportation.Testing.Docs.GettingStarted
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