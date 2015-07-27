using System;

namespace ServiceBusSerenitySamples.Setup
{
    public class MultipleEndpointsSettings
    {
        // For any properties that match the TestRegistry, the in-memory setup 
        // will ensure the URIs match up.
        public Uri AnotherService { get; set; }
        public Uri Client { get; set; }
        public Uri SystemUnderTest { get; set; }
    }
}