using System;
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
        private readonly List<BehaviorChain> _behaviors = new List<BehaviorChain>();

        private readonly SettingsCollection _settings;

        public IRoutePolicy RoutePolicy = new StandardRoutePolicy();

        public BehaviorGraph(SettingsCollection settings) 
        {
            _settings = settings;
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

        public IEnumerable<IRouteDefinition> Routes
        {
            get { return _behaviors.OfType<RoutedChain>().Select(x => x.Route); }
        }

        public IEnumerable<HandlerChain> Handlers
        {
            get { return _behaviors.OfType<HandlerChain>(); }
        } 

        /// <summary>
        ///   All the BehaviorChain's
        /// </summary>
        public IEnumerable<BehaviorChain> Behaviors
        {
            get { return _behaviors; }
        }

        void IChainImporter.Import(IEnumerable<BehaviorChain> chains)
        {
            chains.Each(AddChain);
        }

        internal Registry ToStructureMapRegistry()
        {
            var registry = new Registry();
            _behaviors.OfType<IContainerModel>().Each(x => registry.For<IActionBehavior>().AddInstance(x.ToInstance()));

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
            var chain = _behaviors.OfType<RoutedChain>().FirstOrDefault(x => x.Route == route);
            if (chain == null)
            {
                chain = new RoutedChain(route);
                _behaviors.Fill(chain);
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
            foreach (var chain in _behaviors)
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
            foreach (var chain in _behaviors)
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
            return _behaviors.Where(x => x.Calls.Contains(call)).FirstOrDefault();
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
            var chains = _behaviors.Where(x => x.Calls.Contains(call));
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
            _behaviors.Each(x => Trace.WriteLine(x.ToString()));
        }

        public void AddChain(BehaviorChain chain)
        {
            _behaviors.Add(chain);
        }

        public void AddChains(IEnumerable<BehaviorChain> chains)
        {
            _behaviors.AddRange(chains);
        }

        public void RemoveChain(BehaviorChain chain)
        {
            _behaviors.Remove(chain);
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
            return Behaviors.FirstOrDefault(x => x.InputType() == inputType);
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
            return Behaviors.OfType<RoutedChain>().FirstOrDefault(x => x.Route.Pattern == string.Empty);
        }

        public static BehaviorGraph BuildEmptyGraph()
        {
            return BuildFrom(new FubuRegistry());
        }

    }


}