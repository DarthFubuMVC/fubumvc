using System;
using FubuCore;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.LightningQueues.Testing
{
    [TestFixture]
    public class UriExtensionsTester
    {
        [Test]
        public void uses_machinename_over_localhost()
        {
            var testUri = new Uri("lq.tcp://localhost:5150/blah/test?querystring=value");
            var uri = testUri.ToMachineUri();
            uri.ToString().ShouldBe("lq.tcp://{0}:5150/blah/test?querystring=value".ToFormat(Environment.MachineName.ToLower()));
        }
    }
}