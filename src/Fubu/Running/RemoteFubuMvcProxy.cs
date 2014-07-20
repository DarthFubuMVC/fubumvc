using System;
using Bottles;
using Bottles.Services.Remote;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Diagnostics;
using FubuMVC.Katana;
using FubuMVC.Spark.SparkModel;
using HtmlTags;
using Spark;

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
            _runner = RemoteServiceRunner.For<RemoteFubuMvcBootstrapper>(x => {
                x.RequireAssemblyContainingType<EmbeddedFubuMvcServer>(AssemblyCopyMode.SemVerCompatible);
                x.RequireAssemblyContainingType<RemoteFubuMvcProxy>(AssemblyCopyMode.SemVerCompatible);
                x.RequireAssemblyContainingType<RemoteServiceRunner>(AssemblyCopyMode.SemVerCompatible);
                x.RequireAssemblyContainingType<Owin.IAppBuilder>();
                x.RequireAssemblyContainingType<IActivator>(); // Bottles
                x.RequireAssemblyContainingType<IModelBinder>(); // FubuCore
                x.RequireAssemblyContainingType<FubuApplication>(AssemblyCopyMode.SemVerCompatible); // FubuMVC.Core
                x.RequireAssemblyContainingType<HtmlTag>(AssemblyCopyMode.SemVerCompatible); // HtmlTags
                x.RequireAssemblyContainingType<FubuCsProjFile.CodeFile>(); // FubuCsProjFile just to keep it from whining
                x.RequireAssemblyContainingType<DiagnosticChainsSource>(AssemblyCopyMode.SemVerCompatible); 
                x.RequireAssemblyContainingType<SparkViewEngine>(AssemblyCopyMode.SemVerCompatible);
                x.RequireAssemblyContainingType<SparkTemplate>(AssemblyCopyMode.SemVerCompatible);

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
                ApplicationName = _request.ApplicationFlag,
                PhysicalPath = _request.DirectoryFlag,
                PortNumber = PortFinder.FindPort(_request.PortFlag),
                UseProductionMode = _request.ProductionModeFlag,
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

        public void GenerateTemplates()
        {
            _runner.SendRemotely(new GenerateTemplates());
        }
    }

    public class GenerateTemplates{}
}