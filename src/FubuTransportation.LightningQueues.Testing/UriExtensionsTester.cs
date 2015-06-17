using System;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuTransportation.LightningQueues.Testing
{
    [TestFixture]
    public class UriExtensionsTester
    {
        [Test]
        public void uses_machinename_over_localhost()
        {
            var testUri = new Uri("lq.tcp://localhost:5150/blah/test?querystring=value");
            var uri = testUri.ToMachineUri();
            uri.ToString().ShouldEqual("lq.tcp://{0}:5150/blah/test?querystring=value".ToFormat(Environment.MachineName.ToLower()));
        }
    }
}