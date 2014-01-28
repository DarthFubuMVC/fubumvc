using System.Collections.Generic;
using System.Threading;
using FubuCore;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    public class OwinClientConnectivity : IClientConnectivity
    {
        private readonly IDictionary<string, object> _environment;

        public OwinClientConnectivity(IDictionary<string, object> environment)
        {
            _environment = environment;
        }

        public bool IsClientConnected()
        {
            var cancellation = _environment.Get<CancellationToken>(OwinConstants.CallCancelledKey);
            return cancellation == null ? false : !cancellation.IsCancellationRequested;
        }
    }
}