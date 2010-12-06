using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore;

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

        public void Facility(PackageFacility facility)
        {
            configure = graph =>
            {
                graph.PushProvenance(facility.ToString());
                facility.As<IPackagingRuntimeGraphConfigurer>().Configure(graph);
                graph.PopProvenance();
            };
        }

        public void Activator(IPackageActivator activator)
        {
            configure = g => g.AddActivator(activator);
        }

        public void Bootstrap(Func<IPackageLog, IEnumerable<IPackageActivator>> lambda)
        {
            Bootstrapper(new LambdaBootstrapper(lambda));
        }

        void IPackagingRuntimeGraphConfigurer.Configure(PackagingRuntimeGraph graph)
        {
            _configurations.Each(x => x(graph));
        }
    }
}