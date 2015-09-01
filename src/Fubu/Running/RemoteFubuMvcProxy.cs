using System;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Services.Remote;
using HtmlTags;
using Owin;

namespace Fubu.Running
{
    public class RemoteFubuMvcProxy : IDisposable
    {
        private readonly ApplicationRequest _request;
        private RemoteServiceRunner _runner;

        public RemoteFubuMvcProxy(ApplicationRequest request)
        {
            _request = request;
        }

        public void Start(object listener, Action<RemoteDomainExpression> configuration = null)
        {
            _runner = RemoteServiceRunner.For<RemoteFubuMvcBootstrapper>(x =>
            {
                x.RequireAssemblyContainingType<RemoteFubuMvcProxy>(AssemblyCopyMode.SemVerCompatible);
                x.RequireAssemblyContainingType<IAppBuilder>();
                x.RequireAssemblyContainingType<IModelBinder>(); // FubuCore
                x.RequireAssemblyContainingType<FubuRuntime>(AssemblyCopyMode.SemVerCompatible); // FubuMVC.Core
                x.RequireAssemblyContainingType<HtmlTag>(AssemblyCopyMode.SemVerCompatible); // HtmlTags

                x.RequireAssembly("Microsoft.Owin.Hosting");
                x.RequireAssembly("Microsoft.Owin.Host.HttpListener");
                x.RequireAssembly("Microsoft.Owin");
                x.RequireAssembly("Owin");

                x.ServiceDirectory = _request.DirectoryFlag;

                if (_request.ConfigFlag.IsNotEmpty())
                {
                    x.Setup.ConfigurationFile = _request.ConfigFlag;
                }

                x.Setup.PrivateBinPath = _request.DetermineBinPath();

                if (configuration != null)
                {
                    configuration(x);
                }

                Console.WriteLine("Assembly bin path is " + x.Setup.PrivateBinPath);
                Console.WriteLine("The configuration file is " + x.Setup.ConfigurationFile);
            });

            _runner.WaitForServiceToStart<RemoteFubuMvcBootstrapper>();

            _runner.Messaging.AddListener(listener);


            _runner.SendRemotely(new StartApplication
            {
                ApplicationName = _request.RegistryFlag,
                PhysicalPath = _request.DirectoryFlag,
                PortNumber = PortFinder.FindPort(_request.PortFlag),
                Mode = _request.ModeFlag,
                AutoRefreshWebSocketsAddress = _request.AutoRefreshWebSocketsAddress
            });
        }

        public void Recycle()
        {
            _runner.SendRemotely(new RecycleApplication());
        }

        public void Dispose()
        {
            _runner.Dispose();
        }
    }
}