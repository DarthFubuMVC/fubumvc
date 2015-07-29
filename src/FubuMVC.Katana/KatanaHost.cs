using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Builder;
using Microsoft.Owin.Hosting.Engine;
using Microsoft.Owin.Hosting.Loader;
using Microsoft.Owin.Hosting.ServerFactory;
using Microsoft.Owin.Hosting.Services;
using Microsoft.Owin.Hosting.Tracing;
using Microsoft.Owin.Logging;

using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace FubuMVC.Katana
{

    public class KatanaHost : IHost
    {
        public IDisposable Start(int port, AppFunc func, IDictionary<string, object> properties)
        {
            var parameters = new StartOptions { Port = port };
            parameters.Urls.Add("http://*:" + port); //for netsh http add urlacl

            var context = new StartContext(parameters)
            {
                App = func
            };


            context.EnvironmentData.AddRange(properties);

            var engine = new HostingEngine(new AppBuilderFactory(), new TraceOutputFactory(),
                new AppLoader(new IAppLoaderFactory[0]),
                new ServerFactoryLoader(new ServerFactoryActivator(new ServiceProvider())),
                new DiagnosticsLoggerFactory());

            try
            {
                return engine.Start(context);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("Access is denied"))
                {
                    throw new KatanaRightsException(e.InnerException);
                }

                throw;
            }
        }
    }
}