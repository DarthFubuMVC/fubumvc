using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Services;
using FubuMVC.Core.StructureMap;
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

        private FubuApplication(FubuRegistry registry, string rootPath = null)
        {
            _registry = registry;
            RootPath = rootPath;
        }

        public string RootPath { get; set; }

        /// <summary>
        /// Use the policies and conventions specified by the registry
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static FubuApplication For(FubuRegistry registry, string rootPath = null)
        {
            return new FubuApplication(registry)
            {
                RootPath = rootPath
            };
        }

        /// <summary>
        /// Use only the default FubuMVC policies and conventions
        /// </summary>
        /// <returns></returns>
        public static FubuApplication DefaultPolicies(IContainer container = null)
        {
            var assembly = FindTheCallingAssembly();
            var registry = new FubuRegistry(assembly);
            if (container != null)
            {
                registry.StructureMap(container);
            }

            return new FubuApplication(registry);

        }

        /// <summary>
        /// Use the policies and conventions specified by a specific FubuRegistry of type "T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FubuApplication For<T>(Action<T> customize = null, string rootPath = null) where T : FubuRegistry, new()
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

            var application = new BasicApplication(_registry)
            {
                RootPath = RootPath
            };

            return new FubuRuntime(application);
        }




        /// <summary>
        ///   Finds the currently executing assembly.
        /// </summary>
        /// <returns></returns>
        public static Assembly FindTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            var thisAssembly = Assembly.GetExecutingAssembly().GetName().Name;
            var fubuCore = typeof (IObjectResolver).Assembly.GetName().Name;

            Assembly callingAssembly = null;
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                var name = assembly.GetName().Name;

                if (name != thisAssembly && name != fubuCore && name != "mscorlib" &&
                    name != "FubuMVC.Katana" && name != "FubuMVC.NOWIN" && name != "Serenity" && name != "System.Core")
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }

        public static readonly Cache<string, string> Properties = new Cache<string, string>(key => null);
    }
}