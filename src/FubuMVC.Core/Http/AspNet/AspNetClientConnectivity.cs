using System;
using System.Web;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetClientConnectivity : IClientConnectivity
    {
        private readonly HttpResponseBase _response;

        public AspNetClientConnectivity(HttpResponseBase response)
        {
            _response = response;
        }

        public bool IsClientConnected()
        {
            return _response.IsClientConnected;
        }
    }
}