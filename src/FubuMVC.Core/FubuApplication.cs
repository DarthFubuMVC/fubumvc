using System;
using System.Web.Routing;
using FubuCore.Util;
using FubuMVC.Core.Services;
using StructureMap;

namespace FubuMVC.Core
{
    // PLEASE NOTE:  This code is primarily tested through the integration tests

    /// <summary>
    /// Key class used to define and bootstrap a FubuMVC application
    /// </summary>
    public class FubuApplication : IApplication<FubuRuntime>
    {
        private readonly FubuRegistry _registry;

        internal FubuApplication(FubuRegistry registry, string rootPath = null)
        {
            _registry = registry;
            RootPath = rootPath;
        }

        public string RootPath { get; set; }

        /// <summary>
        /// Use the policies and conventions specified by a specific FubuRegistry of type "T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FubuApplication For<T>(Action<T> customize = null, string rootPath = null)
            where T : FubuRegistry, new()
        {
            var registry = new T();
            if (customize != null)
            {
                customize(registry);
            }

            return new FubuApplication(registry)
            {
                RootPath = rootPath
            };
        }


        /// <summary>
        /// Shortcut method to immediately bootstrap the specified FubuMVC application
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FubuRuntime BootstrapApplication<T>() where T : IApplicationSource, new()
        {
            return new T().BuildApplication().Bootstrap();
        }

        /// <summary>
        /// Called to bootstrap and "start" a FubuMVC application 
        /// </summary>
        /// <returns></returns>
        public FubuRuntime Bootstrap()
        {
            RouteTable.Routes.Clear();

            _registry.RootPath = RootPath;

            return new FubuRuntime(_registry);
        }


        public static readonly Cache<string, string> Properties = new Cache<string, string>(key => null);
    }
}