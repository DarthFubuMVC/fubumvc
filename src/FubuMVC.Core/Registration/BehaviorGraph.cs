using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace FubuMVC.Core.Registration
{
    public interface IChainImporter
    {
        void Import(IEnumerable<BehaviorChain> chains);
    }


    /// <summary>
    ///   The complete behavior model of a fubu application
    /// </summary>
    public class BehaviorGraph : IChainImporter
    {
        private readonly List<BehaviorChain> _chains = new List<BehaviorChain>();

        private readonly SettingsCollection _settings;

        public IRoutePolicy RoutePolicy = new StandardRoutePolicy();

        public BehaviorGraph(SettingsCollection settings) 
        {
            _settings = settings;
            Routes = new RouteCollection(this);
        }

        public BehaviorGraph() : this(new SettingsCollection())
        {
            _settings.Replace(SessionStateRequirement.RequiresSessionState);

        }

        public Assembly ApplicationAssembly { get; set; }
        public IEnumerable<Assembly> PackageAssemblies { get; set; }

        public IEnumerable<Assembly> AllAssemblies()
        {
            if (ApplicationAssembly != null) yield return ApplicationAssembly;

            if (PackageAssemblies != null)
            {
                foreach (var packageAssembly in PackageAssemblies)
                {
                    yield return packageAssembly;
                }
            }
        } 

        private string _version;

        public string Version
        {
            get
            {
                return _version.IsEmpty()
                    ? (ApplicationAssembly == null ? string.Empty : ApplicationAssembly.GetName().Version.ToString())
                    : _version;
            }
            set { _version = value; }
        }


        public SettingsCollection Settings
        {
            get { return _settings; }
        }

        public readonly RouteCollection Routes;


        public IEnumerable<HandlerChain> Handlers
        {
            get { return _chains.OfType<HandlerChain>(); }
        }

        public IEnumerable<PollingJobChain> PollingJobs
        {
            get { return _chains.OfType<PollingJobChain>(); }
        } 

        /// <summary>
        ///   All the BehaviorChain's
        /// </summary>
        public IEnumerable<BehaviorChain> Chains
        {
            get { return _chains; }
        }

        void IChainImporter.Import(IEnumerable<BehaviorChain> chains)
        {
            chains.Each(AddChain);
        }

        internal Registry ToStructureMapRegistry()
        {
            var registry = new Registry();
            _chains.OfType<IContainerModel>().Each(x => registry.For<IActionBehavior>().AddInstance(x.ToInstance()));

            return registry;
        }


        public static BehaviorGraph BuildFrom(FubuRegistry registry, IPerfTimer timer = null)
        {
            return BehaviorGraphBuilder.Build(registry, timer ?? new PerfTimer(), new Assembly[0], new ActivationDiagnostics(), FubuApplicationFiles.ForDefault());
        }

        public static BehaviorGraph BuildFrom<T>(IPerfTimer timer = null) where T : FubuRegistry, new()
        {
            return BehaviorGraphBuilder.Build(new T(), timer ?? new PerfTimer(), new Assembly[0], new ActivationDiagnostics(), FubuApplicationFiles.ForDefault());
        }

        public static BehaviorGraph BuildFrom(Action<FubuRegistry> configure, IPerfTimer timer = null)
        {
            var registry = new FubuRegistry();
            configure(registry);

            return BehaviorGraphBuilder.Build(registry, timer ?? new PerfTimer(), new Assembly[0], new ActivationDiagnostics(), FubuApplicationFiles.ForDefault());
        }


        /// <summary>
        ///   Finds the matching BehaviorChain for the given IRouteDefinition.  If no
        ///   BehaviorChain exists, one is created and added to the BehaviorGraph
        /// </summary>
        /// <param name = "route"></param>
        /// <returns></returns>
        public BehaviorChain ChainFor(IRouteDefinition route)
        {
            var chain = _chains.OfType<RoutedChain>().FirstOrDefault(x => x.Route == route);
            if (chain == null)
            {
                chain = new RoutedChain(route);
                _chains.Fill(chain);
            }

            return chain;
        }


        /// <summary>
        ///   All of the actions in all of the BehaviorChains
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ActionCall> Actions()
        {
            return allActions().ToList();
        }

        /// <summary>
        ///   An enumeration of all the "FirstCall's" in the
        ///   BehaviorGraph across all BehaviorChains
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ActionCall> FirstActions()
        {
            foreach (var chain in _chains)
            {
                var call = chain.FirstCall();
                if (call != null)
                {
                    yield return call;
                }
            }
        }

        private IEnumerable<ActionCall> allActions()
        {
            foreach (var chain in _chains)
            {
                foreach (var call in chain.Calls)
                {
                    yield return call;
                }
            }
        }

        /// <summary>
        ///   Finds the *first* BehaviorChain that contains an
        ///   ActionCall for the Method designated by the expression
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "expression"></param>
        /// <returns></returns>
        public BehaviorChain ChainFor<T>(Expression<Action<T>> expression)
        {
            var call = ActionCall.For(expression);
            return _chains.Where(x => x.Calls.Contains(call)).FirstOrDefault();
        }

        /// <summary>
        ///   Finds the *first* BehaviorChain that contains an
        ///   ActionCall for the Method designated by the expression
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "expression"></param>
        /// <returns></returns>
        public BehaviorChain ChainFor<T>(Expression<Func<T, object>> expression)
        {
            var call = ActionCall.For(expression);
            var chains = _chains.Where(x => x.Calls.Contains(call));
            if (chains.Count() > 1)
            {
                throw new FubuException(1020,
                    "More than one behavior chain contains this ActionCall.  You will have to use a more specific search");
            }

            return chains.FirstOrDefault();
        }

        /// <summary>
        ///   Writes a summary of the BehaviorGraph to Trace
        /// </summary>
        public void Describe()
        {
            _chains.Each(x => Trace.WriteLine(x.ToString()));
        }

        public void AddChain(BehaviorChain chain)
        {
            _chains.Add(chain);
        }

        public void AddChains(IEnumerable<BehaviorChain> chains)
        {
            _chains.AddRange(chains);
        }

        public void RemoveChain(BehaviorChain chain)
        {
            _chains.Remove(chain);
        }


        /// <summary>
        ///   Adds a new blank BehaviorChain to the BehaviorGraph
        /// </summary>
        /// <returns></returns>
        public BehaviorChain AddChain()
        {
            var chain = new BehaviorChain();
            AddChain(chain);

            return chain;
        }

        /// <summary>
        ///   Finds the single BehaviorChain with the designated inputType.
        ///   Behaviors.Single(x => x.InputType() == inputType)
        /// </summary>
        /// <param name = "inputType"></param>
        /// <returns></returns>
        public BehaviorChain ChainFor(Type inputType)
        {
            return Chains.FirstOrDefault(x => x.InputType() == inputType);
        }

        /// <summary>
        /// Find the first BehaviorChain with InputType() == typeof(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BehaviorChain ChainFor<T>()
        {
            return ChainFor(typeof (T));
        }

        public BehaviorChain FindHomeChain()
        {
            return Chains.OfType<RoutedChain>().FirstOrDefault(x => x.Route.Pattern == string.Empty);
        }

        public static BehaviorGraph BuildEmptyGraph()
        {
            return BuildFrom(new FubuRegistry());
        }

        public BehaviorChain FindChain(int key)
        {
            return Chains.FirstOrDefault(x => x.Key == key);
        }
    }

    public class RouteCollection : IEnumerable<RoutedChain>
    {
        private readonly BehaviorGraph _graph;

        public RouteCollection(BehaviorGraph graph)
        {
            _graph = graph;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<RoutedChain> GetEnumerator()
        {
            return _graph.Chains.OfType<RoutedChain>().GetEnumerator();
        }

        public IEnumerable<RoutedChain> Gets
        {
            get { return this.Where(x => x.Route.RespondsToMethod("GET")); }
        }


        public IEnumerable<RoutedChain> Posts
        {
            get { return this.Where(x => x.Route.RespondsToMethod("POST")); }
        }


        public IEnumerable<RoutedChain> Puts
        {
            get { return this.Where(x => x.Route.RespondsToMethod("PUT")); }
        }


        public IEnumerable<RoutedChain> Deletes
        {
            get { return this.Where(x => x.Route.RespondsToMethod("DELETE")); }
        }


        public IEnumerable<RoutedChain> Heads
        {
            get { return this.Where(x => x.Route.RespondsToMethod("HEAD")); }
        }

        /// <summary>
        /// Union of routed chains that respond to GET or HEAD
        /// </summary>
        public IEnumerable<RoutedChain> Resources
        {
            get { return Gets.Union(Heads); }
        }

        /// <summary>
        /// Union of routed chains that respond to POST, PUT, or DELETE
        /// </summary>
        public IEnumerable<RoutedChain> Commands
        {
            get { return Posts.Union(Puts).Union(Deletes); }
        } 
    }
}