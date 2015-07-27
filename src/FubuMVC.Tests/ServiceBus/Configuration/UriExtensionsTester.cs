using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Configuration;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    [TestFixture]
    public class UriExtensionsTester
    {
        [Test]
        public void can_handle_localhost_consistently()
        {
            var uri = new Uri("http://lOcAlHoSt/blahdee");
            var normalizedUri = uri.NormalizeLocalhost();

            normalizedUri.ToString().ShouldBe("http://{0}/blahdee".ToFormat(System.Environment.MachineName.ToLower()));
        }

        [Test]
        public void can_handle_localhost_consistently_2()
        {
            var uri = new Uri("http://127.0.0.1/blahdee");
            var normalizedUri = uri.NormalizeLocalhost();
            normalizedUri.ToString().ShouldBe("http://{0}/blahdee".ToFormat(System.Environment.MachineName.ToLower()));
        }
    }
}