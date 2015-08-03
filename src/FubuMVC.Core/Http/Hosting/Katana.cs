using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;

namespace FubuMVC.Core.Http.Hosting
{
    public class Katana : IHost
    {
        public IDisposable Start(int port, Func<IDictionary<string, object>, Task> func,
            IDictionary<string, object> properties)
        {
            var parameters = build("Microsoft.Owin.Hosting.StartOptions, Microsoft.Owin.Hosting");
            parameters.SetProperty("Port", port);

            parameters.GetProperty("Urls").Call("Add", "http://*:" + port);

            var context = build("Microsoft.Owin.Hosting.Engine.StartContext, Microsoft.Owin.Hosting", parameters);
            context.SetProperty("App", func);
            context.GetProperty("EnvironmentData").Call("AddRange", properties);

            var appBuilderFactory = build("Microsoft.Owin.Hosting.Builder.AppBuilderFactory, Microsoft.Owin.Hosting");
            var traceOutputFactory = build("Microsoft.Owin.Hosting.Tracing.TraceOutputFactory, Microsoft.Owin.Hosting");
            var appLoaderFactories = createAppLoaderFactories();

            var appLoader = build("Microsoft.Owin.Hosting.Loader.AppLoader, Microsoft.Owin.Hosting", appLoaderFactories);
            var serviceProvider = build("Microsoft.Owin.Hosting.Services.ServiceProvider, Microsoft.Owin.Hosting");
            var serverFactoryActivator =
                build("Microsoft.Owin.Hosting.ServerFactory.ServerFactoryActivator, Microsoft.Owin.Hosting",
                    serviceProvider);

            var serverFactoryLoader =
                build("Microsoft.Owin.Hosting.ServerFactory.ServerFactoryLoader, Microsoft.Owin.Hosting",
                    serverFactoryActivator);

            var diagnosticsLoggerFactory = build("Microsoft.Owin.Logging.DiagnosticsLoggerFactory, Microsoft.Owin");

            var engine = build("Microsoft.Owin.Hosting.Engine.HostingEngine, Microsoft.Owin.Hosting",
                appBuilderFactory, traceOutputFactory, appLoader, serverFactoryLoader, diagnosticsLoggerFactory);


            try
            {
                return engine.Call("Start", context).As<IDisposable>();
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("Access is denied"))
                {
                    throw new AdminRightsException(e.InnerException);
                }

                throw;
            }
        }

        private static Array createAppLoaderFactories()
        {
            var appLoaderFactories =
                Array.CreateInstance(
                    Type.GetType("Microsoft.Owin.Hosting.Loader.IAppLoaderFactory, Microsoft.Owin.Hosting"), 0);
            return appLoaderFactories;
        }

        private object build(string typeName, params object[] args)
        {
            var type = Type.GetType(typeName);
            return Activator.CreateInstance(type, args);
        }
    }
}