using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public class PackagingRuntimeGraph
    {
        private readonly IList<IActivator> _activators = new List<IActivator>();
        private readonly IAssemblyLoader _assemblies;
        private readonly IList<IBootstrapper> _bootstrappers = new List<IBootstrapper>();
        private readonly IPackagingDiagnostics _diagnostics;
        private readonly IList<IPackageLoader> _loaders = new List<IPackageLoader>();
        private readonly Stack<string> _provenanceStack = new Stack<string>();
        private List<IPackageInfo> _packages;

        public PackagingRuntimeGraph(IPackagingDiagnostics diagnostics, IAssemblyLoader assemblies)
        {
            _diagnostics = diagnostics;
            _assemblies = assemblies;
        }

        private string currentProvenance
        {
            get { return _provenanceStack.Peek(); }
        }


        public void PushProvenance(string provenance)
        {
            _provenanceStack.Push(provenance);
        }

        public void PopProvenance()
        {
            _provenanceStack.Pop();
        }

        public IEnumerable<IPackageInfo> DiscoverAndLoadPackages(Action onAssembliesScanned)
        {
            _packages = findAllPackages();
            loadAssemblies(_packages, onAssembliesScanned);
            var discoveredActivators = runAllBootstrappers();
            activatePackages(_packages, discoveredActivators);

            return _packages;
        }

        private void activatePackages(List<IPackageInfo> packages, List<IActivator> discoveredActivators)
        {
            _diagnostics.LogExecutionOnEach(discoveredActivators.Union(_activators),
                                            a => { a.Activate(packages, _diagnostics.LogFor(a)); });
        }

        private List<IActivator> runAllBootstrappers()
        {
            var discoveredActivators = new List<IActivator>();
            _diagnostics.LogExecutionOnEach(_bootstrappers, b =>
            {
                var bootstrapperActivators = b.Bootstrap(_diagnostics.LogFor(b));
                discoveredActivators.AddRange(bootstrapperActivators);
                _diagnostics.LogBootstrapperRun(b, bootstrapperActivators);
            });
            return discoveredActivators;
        }

        private void loadAssemblies(IEnumerable<IPackageInfo> packages, Action onAssembliesScanned)
        {
            _diagnostics.LogExecutionOnEach(packages, _assemblies.ReadPackage);

            onAssembliesScanned();
        }

        private List<IPackageInfo> findAllPackages()
        {
            var packages = new List<IPackageInfo>();
            _diagnostics.LogExecutionOnEach(_loaders, loader =>
            {
                var packageInfos = loader.Load();
                _diagnostics.LogPackages(loader, packageInfos);
                packages.AddRange(packageInfos);
            });
            return packages;
        }

        public void AddBootstrapper(IBootstrapper bootstrapper)
        {
            _bootstrappers.Add(bootstrapper);
            _diagnostics.LogObject(bootstrapper, currentProvenance);
        }

        public void AddLoader(IPackageLoader loader)
        {
            _loaders.Add(loader);
            _diagnostics.LogObject(loader, currentProvenance);
        }

        public void AddActivator(IActivator activator)
        {
            _activators.Add(activator);
            _diagnostics.LogObject(activator, currentProvenance);
        }

        public void AddFacility(IPackageFacility facility)
        {
            _diagnostics.LogObject(facility, currentProvenance);

            PushProvenance(facility.ToString());
            
            facility.As<IPackagingRuntimeGraphConfigurer>().Configure(this);
            PopProvenance();
        }
    }
}