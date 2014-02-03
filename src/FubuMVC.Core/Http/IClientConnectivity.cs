using System;

namespace FubuMVC.Core.Http
{
    /// <summary>
    /// Used to test whether or not a client is actively
    /// connected to the request
    /// </summary>
    // TODO -- fold this into ICurrentHttpRequest in 2.0
    public interface IClientConnectivity
    {
        bool IsClientConnected();
    }

    public class StandInClientConnectivity : IClientConnectivity
    {
        public bool IsClientConnected()
        {
            return true;
        }
    }
}