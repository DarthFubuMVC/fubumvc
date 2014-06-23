using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Services;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;

namespace FubuMVC.Core.Configuration
{
    public class PolicyGraph
    {
        public readonly ConfigurationActionSet Policies = new ConfigurationActionSet();
        public readonly ConfigurationActionSet Explicits = new ConfigurationActionSet();
        
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

        public readonly IList<ISettingsAlteration> Settings = new List<ISettingsAlteration>();

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

        // Tested through the FubuRegistry.BuildLocal()
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

        public IEnumerable<IChainSource> Sources
        {
            get { return _sources; }
        }

        public void ApplyGlobalReorderings(BehaviorGraph graph)
        {
            Global.Reordering.RunActions(graph);

            GlobalReorderingRules().Each(x => x.Configure(graph));
        }

        public IEnumerable<IConfigurationAction> GlobalReorderingRules()
        {
            yield return new OutputBeforeAjaxContinuationPolicy();

            yield return new ReorderBehaviorsPolicy
            {
                CategoryMustBeAfter = BehaviorCategory.Authorization,
                CategoryMustBeBefore = BehaviorCategory.Authentication
            };

            yield return new ReorderBehaviorsPolicy()
                .ThisNodeMustBeBefore<OutputCachingNode>()
                .ThisNodeMustBeAfter<OutputNode>();

        }

        public void RegisterServices(BehaviorGraph graph)
        {
            graph.Settings.Register(graph.Services);

            AllServiceRegistrations().Union(DefaultServices())
                .OfType<IServiceRegistration>()
                .Each(x => x.Apply(graph.Services));

            graph.Services.AddService(this);
        }

        public static IEnumerable<ServiceRegistry> DefaultServices()
        {
            yield return new ModelBindingServicesRegistry();
            yield return new SecurityServicesRegistry();
            yield return new HttpStandInServiceRegistry();
            yield return new CoreServiceRegistry();
            yield return new CachingServiceRegistry();
            yield return new UIServiceRegistry();
        }

        public void BuildLocal(BehaviorGraph graph)
        {
            // Local policies will ONLY apply to chains built by this ConfigGraph,
            // and not to chains that are built by imports

            var imports = UniqueImports().Select(x => {
                return Task.Factory.StartNew(() => {
                    return x.BuildChains(graph);
                });
            }).ToArray();

            var chainSources = Sources.Select(source => {
                return Task.Factory.StartNew(() => {
                    return source.BuildChains(graph);
                });
            }).ToArray();

            Task.WaitAll(chainSources);

            chainSources.Each(x => graph.AddChains(x.Result));

            Local.Explicits.RunActions(graph);
            Local.Policies.RunActions(graph);
            Local.Reordering.RunActions(graph);

            Task.WaitAll(imports);

            imports.Each(x => graph.AddChains(x.Result));


        }

        public void ImportGlobals(ConfigGraph config)
        {
            Global.Explicits.Import(config.Global.Explicits);
            Global.Policies.Import(config.Global.Policies);
            Global.Reordering.Import(config.Global.Reordering);
        }
    }
}