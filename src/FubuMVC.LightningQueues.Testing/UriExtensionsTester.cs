using System;
using FubuCore;
using Xunit;
using Shouldly;

namespace FubuMVC.LightningQueues.Testing
{
    
    public class UriExtensionsTester
    {
        private readonly string MachineName = Environment.MachineName.ToLower();

        [Theory]
        [InlineData("127.0.0.1")]
        [InlineData("localhost")]
        public void machineuri_uses_machinename_over_localhost(string host)
        {
            var testUri = new Uri($"lq.tcp://{host}:5150/blah/test?querystring=value");
            var uri = testUri.ToMachineUri();
            uri.ToString().ShouldBe("lq.tcp://{0}:5150/blah/test?querystring=value".ToFormat(Environment.MachineName.ToLower()));
        }

        [Theory]
        [InlineData("10.20.30.40", false)]
        [InlineData("127.0.0.2", false)] //Other IPs in 127.0.0.0/8 aren't recognized.
        [InlineData("some.name", true)]
        [InlineData("127.0.0.1", true)]
        [InlineData("localhost", true)]
        public void localuri_replaces_hosts_with_machinename_unless_an_ip(string host, bool shouldBeMachineName)
        {
            var testUri = new Uri($"lq.tcp://{host}:5150/blah/test?querystring=value");
            var machineUri = testUri.ToLocalUri();
            MachineName.Equals(machineUri.Host).ShouldBe(shouldBeMachineName);
        }
    }
}
