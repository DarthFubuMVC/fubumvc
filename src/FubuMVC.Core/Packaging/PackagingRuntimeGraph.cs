using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Packaging
{
    public class PackagingRuntimeGraph
    {
        private readonly IList<IPackageActivator> _activators = new List<IPackageActivator>();
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
            get { return _provenanceStack.Reverse().Join("/"); }
        }


        public void PushProvenance(string provenance)
        {
            _provenanceStack.Push(provenance);
        }

        public void PopProvenance()
        {
            _provenanceStack.Pop();
        }




        public void DiscoverAndLoadPackages()
        {
            _packages = findAllPackages();
            loadAssemblies(_packages);
            var discoveredActivators = runAllBootstrappers();
            activatePackages(_packages, discoveredActivators);
        }

        private void activatePackages(List<IPackageInfo> packages, List<IPackageActivator> discoveredActivators)
        {
            _diagnostics.LogExecutionOnEach(discoveredActivators.Union(_activators),
                                            a => { a.Activate(packages, _diagnostics.LogFor(a)); });
        }

        private List<IPackageActivator> runAllBootstrappers()
        {
            var discoveredActivators = new List<IPackageActivator>();
            _diagnostics.LogExecutionOnEach(_bootstrappers, b =>
            {
                var bootstrapperActivators = b.Bootstrap(_diagnostics.LogFor(b));
                discoveredActivators.AddRange(bootstrapperActivators);
                _diagnostics.LogBootstrapperRun(b, bootstrapperActivators);
            });
            return discoveredActivators;
        }

        private void loadAssemblies(List<IPackageInfo> packages)
        {
            _diagnostics.LogExecutionOnEach(packages, p => p.LoadAssemblies(_assemblies));
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

        public void AddActivator(IPackageActivator activator)
        {
            _activators.Add(activator);
            _diagnostics.LogObject(activator, currentProvenance);
        }
    }
}