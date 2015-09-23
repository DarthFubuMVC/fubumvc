using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Services;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Registration;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace FubuMVC.Core.Registration
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
        private readonly IList<Registry> _services = new List<Registry>();

        private readonly ActionSourceAggregator _actionSourceAggregator;
        private readonly HandlerGraphSource _handlers = new HandlerGraphSource();
        private readonly IList<IChainSource> _sources = new List<IChainSource>();

        public readonly PolicyGraph Global = new PolicyGraph();
        public readonly PolicyGraph Local = new PolicyGraph();

        public readonly IList<ISettingsAlteration> Settings = new List<ISettingsAlteration>();

        public readonly ServiceRegistry ApplicationServices = new ServiceRegistry();

        public ConfigGraph(Assembly applicationAssembly)
        {
            _applicationAssembly = applicationAssembly;
            _actionSourceAggregator = new ActionSourceAggregator(_applicationAssembly);

            _sources.Add(_actionSourceAggregator);
            _sources.Add(_handlers);
            _sources.Add(new ActionlessViewChainSource());
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

        public HandlerGraphSource Handlers
        {
            get { return _handlers; }
        }

        public ActionSourceAggregator Actions
        {
            get { return _actionSourceAggregator; }
        }

        public void Add(IChainSource source)
        {
            _sources.Add(source);
        }

        public void Add(Registry services)
        {
            _services.Add(services);
        }

        public void Add(IHandlerSource source)
        {
            _handlers.HandlerSources.Add(source);
        }

        public void Add(IActionSource source)
        {
            _actionSourceAggregator.Sources.Add(source);
        }

        public IEnumerable<Registry> AllServiceRegistrations()
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

            yield return new ReorderBehaviorsPolicy
            {
                WhatMustBeBefore = x => x is ExceptionHandlerNode,
                WhatMustBeAfter = x => true
            };
        }

        public void RegisterServices(string mode, IContainer container, BehaviorGraph graph)
        {
            graph.Settings.Register(ApplicationServices);

            container.Configure(_ =>
            {
                DefaultServices(mode).Each(_.AddRegistry);
                AllServiceRegistrations().Each(_.AddRegistry);

                _.AddRegistry(ApplicationServices);

                _.For<BehaviorGraph>().Use(graph);

                _.AddRegistry(graph.ToStructureMapRegistry());
            });
        }

        public static IEnumerable<ServiceRegistry> DefaultServices(string mode)
        {
            yield return new ModelBindingServicesRegistry();
            yield return new SecurityServicesRegistry();
            yield return new HttpStandInServiceRegistry();
            yield return new CoreServiceRegistry(mode);
        }

        public void BuildLocal(BehaviorGraph graph, IPerfTimer timer)
        {
            // Local policies will ONLY apply to chains built by this ConfigGraph,
            // and not to chains that are built by imports

            var imports = UniqueImports().Select(x => x.BuildChains(graph, timer)).ToArray();


            var chainSources =
                Sources.Select(
                    source => source.BuildChains(graph, timer)).ToArray();

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