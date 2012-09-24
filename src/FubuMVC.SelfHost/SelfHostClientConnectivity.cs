using FubuMVC.Core.Http;

namespace FubuMVC.SelfHost
{
    public class SelfHostClientConnectivity : IClientConnectivity
    {
        public bool IsClientConnected()
        {
            return false; // we simply don't know
        }
    }
}