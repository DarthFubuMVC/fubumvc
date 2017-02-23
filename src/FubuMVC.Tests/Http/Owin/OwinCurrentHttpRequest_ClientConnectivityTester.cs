using System.Collections.Generic;
using System.Threading;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Owin
{
    
    public class OwinCurrentHttpRequest_ClientConnectivityTester
    {
        private CancellationToken theToken;
        private OwinHttpRequest theConnectivity;
        private CancellationTokenSource source;

        public OwinCurrentHttpRequest_ClientConnectivityTester()
        {
            var dictionary = new Dictionary<string,object>();

            source = new CancellationTokenSource();

            theToken = source.Token;

            dictionary.Add(OwinConstants.CallCancelledKey, theToken);

            theConnectivity = new OwinHttpRequest(dictionary);
        }

        [Fact]
        public void the_call_is_connected_if_the_cancellation_token_is_not_signaled()
        {
            theConnectivity.IsClientConnected().ShouldBeTrue();
        }

        [Fact]
        public void the_call_is_not_connected_if_the_token_has_been_signaled()
        {
            source.Cancel();
            theConnectivity.IsClientConnected().ShouldBeFalse();
        }
    }
}