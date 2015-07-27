using System.Collections.Generic;

namespace ServiceBusSerenitySamples.SystemUnderTest
{
    public class MessageRecorder
    {
        public readonly IList<object> Messages = new List<object>();
    }
}