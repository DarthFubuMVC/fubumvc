using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Configuration
{
    public class PolicyGraph
    {
        public readonly ConfigurationActionSet Policies = new ConfigurationActionSet();
        public readonly ConfigurationActionSet Explicits = new ConfigurationActionSet();
        public readonly ConfigurationActionSet Settings = new ConfigurationActionSet();
        public readonly ConfigurationActionSet Reordering = new ConfigurationActionSet();
    }

    /// <summary>
    /// Holds and tracks all the IConfigurationAction's used to construct the BehaviorGraph of an application
    /// </summary>
    public class ConfigGraph
    {
        private readonly Assembly _applicationAssembly;

        private readonly IList<RegistryImport> _imports = new List<RegistryImport>();
        private readonly IList<ServiceRegistry> _services = new List<ServiceRegistry>();

        private readonly ActionSourceAggregator _actionSourceAggregator;
        private readonly IList<IChainSource> _sources = new List<IChainSource>();

        public readonly PolicyGraph Global = new PolicyGraph();
        public readonly PolicyGraph Local = new PolicyGraph();

        public ConfigGraph(Assembly applicationAssembly)
        {
            _applicationAssembly = applicationAssembly;
            _actionSourceAggregator = new ActionSourceAggregator(_applicationAssembly);

            _sources.Add(_actionSourceAggregator);
        }

        public Assembly ApplicationAssembly
        {
            get { return _applicationAssembly; }
        }

        public IEnumerable<RegistryImport> Imports
        {
            get { return _imports; }
        }

        public void AddImport(RegistryImport import)
        {
            if (HasImported(import.Registry)) return;

            _imports.Add(import);
        }

        // Tested through the FubuRegistry.Import()
        public bool HasImported(FubuRegistry registry)
        {
            if (_imports.Any(x => x.Registry.GetType() == registry.GetType()))
            {
                return true;
            }

            if (_imports.Any(x => x.Registry.Config.HasImported(registry)))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<RegistryImport> UniqueImports()
        {
            var children = allChildrenImports().ToList();

            return _imports.Where(x => !children.Contains(x));
        }

        private IEnumerable<RegistryImport> allChildrenImports()
        {
            foreach (var import in _imports)
            {
                foreach (var action in import.Registry.Config._imports)
                {
                    yield return action;

                    foreach (
                        var descendentAction in
                            _imports.SelectMany(x => x.Registry.Config.allChildrenImports()))
                    {
                        yield return descendentAction;
                    }
                }
            }
        }


        public void Add(IChainSource source)
        {
            _sources.Add(source);
        }

        public void Add(ServiceRegistry services)
        {
            _services.Add(services);
        }

        public void Add(IActionSource source)
        {
            _actionSourceAggregator.Add(source);
        }

        public void RegisterServices(ServiceGraph services)
        {
            AllServiceRegistrations().OfType<IServiceRegistration>().Each(x => x.Apply(services));
        }

        public IEnumerable<ServiceRegistry> AllServiceRegistrations()
        {
            foreach (var import in UniqueImports())
            {
                foreach (var log in import.Registry.Config.AllServiceRegistrations())
                {
                    yield return log;
                }
            }

            foreach (var registry in _services)
            {
                yield return registry;
            }
        }

        public static string DetermineConfigurationType(IConfigurationAction action)
        {
            if (action.GetType().HasAttribute<ConfigurationTypeAttribute>())
            {
                return action.GetType().GetAttribute<ConfigurationTypeAttribute>().Type;
            }

            return null;
        }

        public IEnumerable<IChainSource> Sources
        {
            get { return _sources; }
        }
    }
}