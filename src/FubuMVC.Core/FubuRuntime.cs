using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Bootstrapping;
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

        public void Dispose()
        {
            Factory.Dispose();
        }
    }
}