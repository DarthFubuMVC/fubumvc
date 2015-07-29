using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Http.Hosting;
using Microsoft.Owin.Builder;
using Nowin;
using Owin;

namespace FubuMVC.Nowin
{
    public class NowinHost : IHost
    {
        public IDisposable Start(int port, Func<IDictionary<string, object>, Task> func,
            IDictionary<string, object> properties)
        {
            var owinBuilder = new AppBuilder();
            OwinServerFactory.Initialize(owinBuilder.Properties);

            owinBuilder.Run(context => func(context.Environment));

            var builder = ServerBuilder.New().SetPort(port).SetOwinApp(owinBuilder.Build());
            return builder.Start();
        }
    }
}