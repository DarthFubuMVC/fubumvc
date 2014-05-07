using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Bottles;
using Bottles.Diagnostics;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    /// <summary>
    /// Represents a running FubuMVC application, with access to the key parts of the application
    /// </summary>
    public class FubuRuntime : IDisposable
    {
        private readonly IContainerFacility _facility;
        private readonly IServiceFactory _factory;
        private readonly IList<RouteBase> _routes;
        private bool _disposed;

        public FubuRuntime(IServiceFactory factory, IContainerFacility facility, IList<RouteBase> routes)
        {
            _factory = factory;
            _facility = facility;
            _routes = routes;
        }

        public IServiceFactory Factory
        {
            get { return _factory; }
        }

        public IContainerFacility Facility
        {
            get { return _facility; }
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
                var log = PackageRegistry.Diagnostics.LogFor(x);

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

            Facility.Shutdown();
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
        public IPackageLog Log { get; set; }
        public void Describe(Description description)
        {
            description.Title = "Deactivator: " + Deactivator;
            description.LongDescription = Log.FullTraceText();
        }
    }
}