using System.Collections.Generic;

namespace FubuTransportation.Serenity.Samples.SystemUnderTest
{
    public class MessageRecorder
    {
        public readonly IList<object> Messages = new List<object>();
    }
}