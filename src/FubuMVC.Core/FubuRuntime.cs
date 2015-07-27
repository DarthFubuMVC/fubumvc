using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Web.Routing;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.StructureMap;
using FubuMVC.Core.StructureMap.Settings;
using StructureMap;

namespace FubuMVC.Core
{
    /// <summary>
    /// Represents a running FubuMVC application, with access to the key parts of the application
    /// </summary>
    public class FubuRuntime : IDisposable
    {
        private readonly IServiceFactory _factory;
        private readonly IContainer _container;
        private readonly IList<RouteBase> _routes;
        private bool _disposed;
        private readonly IFubuApplicationFiles _files;

        public FubuRuntime(IServiceFactory factory, IContainer container, IList<RouteBase> routes, IFubuApplicationFiles files)
        {
            _factory = factory;
            _container = container;
            _files = files;

            // TODO -- Temporary? AND THIS HAS TO BE LAZY BUILT PER NESTED CONTAINER
            _container.Configure(_ =>
            {
                _.Policies.OnMissingFamily<SettingPolicy>();

                _.For<IFubuApplicationFiles>().Use(files);
                _.For<IServiceLocator>().Use<StructureMapServiceLocator>();
                _.For<FubuRuntime>().Use(this);
                _.For<IServiceFactory>().Use(factory);
            });

            _routes = routes;
        }

        internal void Activate()
        {
            
        }

        public IFubuApplicationFiles Files
        {
            get { return _files; }
        }


        public IContainer Container
        {
            get { return _container; }
        }

        public IServiceFactory Factory
        {
            get { return _factory; }
        }

        public IList<RouteBase> Routes
        {
            get { return _routes; }
        }

        public BehaviorGraph Behaviors
        {
            get
            {
                return Factory.Get<BehaviorGraph>();
            }
        }

        public void Dispose()
        {
            dispose();
            GC.SuppressFinalize(this);
        }

        private void dispose()
        {
             if (_disposed) return;

            _disposed = true;

            var logger = _factory.Get<ILogger>();
            var deactivators = _factory.GetAll<IDeactivator>().ToArray();
            

            deactivators.Each(x => {
                var log = Behaviors.Diagnostics.LogFor(x);

                try
                {
                    x.Deactivate(log);
                }
                catch (Exception e)
                {
                    logger.Error("Failed while running Deactivator", e);
                    log.MarkFailure(e);
                }
                finally
                {
                    logger.InfoMessage(() => new DeactivatorExecuted { Deactivator = x.ToString(), Log = log});
                }
            });

            Container.Dispose();
        }

        ~FubuRuntime()
        {
            try
            {
                dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the finalizer {0}", ex);
            }
        }
    }

    public class DeactivatorExecuted : LogRecord, DescribesItself
    {
        public string Deactivator { get; set; }
        public IActivationLog Log { get; set; }
        public void Describe(Description description)
        {
            description.Title = "Deactivator: " + Deactivator;
            description.LongDescription = Log.FullTraceText();
        }
    }
}