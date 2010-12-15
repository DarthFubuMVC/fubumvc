using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Packaging
{


    public class PackageFacility : IPackageFacility, IPackagingRuntimeGraphConfigurer
    {
        private readonly IList<Action<PackagingRuntimeGraph>> _configurations =
            new List<Action<PackagingRuntimeGraph>>();

        private Action<PackagingRuntimeGraph> configure
        {
            set { _configurations.Add(value); }
        }

        public void Assembly(Assembly assembly)
        {
            configure = g => g.AddLoader(new AssemblyPackageLoader(assembly));
        }

        public void Bootstrapper(IBootstrapper bootstrapper)
        {
            configure = g => g.AddBootstrapper(bootstrapper);
        }

        public void Loader(IPackageLoader loader)
        {
            configure = g => g.AddLoader(loader);
        }

        public void Facility(IPackageFacility facility)
        {
            configure = graph =>
            {
                graph.AddFacility(facility);
                

            };
        }

        public void Activator(IActivator activator)
        {
            configure = g => g.AddActivator(activator);
        }

        public void Bootstrap(Func<IPackageLog, IEnumerable<IActivator>> lambda)
        {
            var lambdaBootstrapper = new LambdaBootstrapper(lambda);
            lambdaBootstrapper.Provenance = findCallToBootstrapper();

            Bootstrapper(lambdaBootstrapper);
        }

        private static string findCallToBootstrapper()
        {
            var packageAssembly = typeof(IPackageInfo).Assembly;
            var trace = new StackTrace(Thread.CurrentThread, false);
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != packageAssembly && !frame.GetMethod().HasAttribute<SkipOverForProvenanceAttribute>())
                {
                    return frame.ToDescription();
                }
            }


            return "Unknown";
        }

        void IPackagingRuntimeGraphConfigurer.Configure(PackagingRuntimeGraph graph)
        {
            _configurations.Each(x => x(graph));
        }
    }
}