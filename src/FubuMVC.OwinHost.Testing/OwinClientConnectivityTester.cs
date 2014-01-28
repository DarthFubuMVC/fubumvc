using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class OwinClientConnectivityTester
    {
        private CancellationToken theToken;
        private OwinClientConnectivity theConnectivity;
        private CancellationTokenSource source;

        [SetUp]
        public void SetUp()
        {
            var dictionary = new Dictionary<string,object>();

            source = new CancellationTokenSource();

            theToken = source.Token;

            dictionary.Add(OwinConstants.CallCancelledKey, theToken);

            theConnectivity = new OwinClientConnectivity(dictionary);
        }

        [Test]
        public void the_call_is_connected_if_the_cancellation_token_is_not_signaled()
        {
            theConnectivity.IsClientConnected().ShouldBeTrue();
        }

        [Test]
        public void the_call_is_not_connected_if_the_token_has_been_signaled()
        {
            source.Cancel();
            theConnectivity.IsClientConnected().ShouldBeFalse();
        }
    }
}