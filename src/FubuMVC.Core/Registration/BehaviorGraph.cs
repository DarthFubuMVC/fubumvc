using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Bottles;
using FubuCore;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Routing;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.Registration
{
    public interface IRegisterable
    {
        void Register(Action<Type, ObjectDef> action);
    }

    public interface IChainImporter
    {
        void Import(IEnumerable<BehaviorChain> chains);
    }


    /// <summary>
    ///   The complete behavior model of a fubu application
    /// </summary>
    public class BehaviorGraph : IRegisterable, IChainImporter
    {
        private readonly List<BehaviorChain> _behaviors = new List<BehaviorChain>();

        private readonly List<IChainForwarder> _forwarders = new List<IChainForwarder>();
        private readonly ServiceGraph _services = new ServiceGraph();

        private readonly SettingsCollection _settings;

        public IRoutePolicy RoutePolicy = new StandardRoutePolicy();

        public BehaviorGraph(SettingsCollection parent) : this()
        {
            _settings = new SettingsCollection(parent);
        }

        public BehaviorGraph()
        {
            _settings = new SettingsCollection(null);
            _settings.Replace(SessionStateRequirement.RequiresSessionState);
            _settings.Replace<IFubuApplicationFiles>(new FubuApplicationFiles());

            RouteIterator = new SortByRouteRankIterator(); // can override in a registry

            TypeResolver = new TypeResolver();
            _services.AddService<ITypeResolver>(TypeResolver);
        }

        public Assembly ApplicationAssembly { get; set; }

        public TypePool Types()
        {
            var types = new TypePool();
            if (ApplicationAssembly != null) types.AddAssembly(ApplicationAssembly);
            types.AddAssemblies(PackageRegistry.PackageAssemblies);

            return types;
        }

        private string _version;
        public string Version
        {
            get
            {
                return _version.IsEmpty()
                    ? (ApplicationAssembly == null ? string.Empty : ApplicationAssembly.GetName().Version.ToString()) : _version;
            }
            set
            {
                _version = value;
            }
        }


        public SettingsCollection Settings
        {
            get { return _settings; }
        }

        public IFubuApplicationFiles Files
        {
            get { return _settings.Get<IFubuApplicationFiles>(); }
        }

        public IList<IChainForwarder> Forwarders
        {
            get { return _forwarders; }
        }

        public TypeResolver TypeResolver { get; private set; }

        public ServiceGraph Services
        {
            get { return _services; }
        }

        public IEnumerable<IRouteDefinition> Routes
        {
            get { return _behaviors.OfType<RoutedChain>().Select(x => x.Route); }
        }

        /// <summary>
        ///   All the BehaviorChain's
        /// </summary>
        public IEnumerable<BehaviorChain> Behaviors
        {
            get { return _behaviors; }
        }

        /// <summary>
        ///   RouteIterator is used to order Routes within the Routing table
        /// </summary>
        public IRouteIterator RouteIterator { get; set; }

        #region IChainImporter Members

        void IChainImporter.Import(IEnumerable<BehaviorChain> chains)
        {
            chains.Each(AddChain);
        }

        #endregion

        #region IRegisterable Members

        void IRegisterable.Register(Action<Type, ObjectDef> action)
        {
            /*
             * 1.) Loop through each service
             * 2.) Loop through each top level behavior for routes
             * 3.) Loop through each partial behavior
             * 4.) add in the UrlRegistry as a value
             */

            _services.Each(action);

            _behaviors.OfType<IRegisterable>().Each(chain => chain.Register(action));

            action(typeof (BehaviorGraph), new ObjectDef
            {
                Value = this
            });
        }

        #endregion

        public static BehaviorGraph BuildFrom(FubuRegistry registry)
        {
            return BehaviorGraphBuilder.Build(registry);
        }

        public static BehaviorGraph BuildFrom<T>() where T : FubuRegistry, new()
        {
            return BehaviorGraphBuilder.Build(new T());
        }

        public static BehaviorGraph BuildFrom(Action<FubuRegistry> configure)
        {
            var registry = new FubuRegistry();
            configure(registry);

            return BehaviorGraphBuilder.Build(registry);
        }

        /// <summary>
        ///   Register a ChainForwarder that forwards UrlFor requests
        ///   for T to something else
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "converter"></param>
        public void Forward<T>(Func<T, object> converter)
        {
            var forwarder = new ChainForwarder<T>(converter);
            _forwarders.Add(forwarder);
        }

        /// <summary>
        ///   Register a ChainForwarder that forwards UrlFor(category) requests
        ///   for T to something else
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "converter"></param>
        /// <param name = "category"></param>
        public void Forward<T>(Func<T, object> converter, string category)
        {
            var forwarder = new ChainForwarder<T>(converter, category);
            _forwarders.Add(forwarder);
        }

        /// <summary>
        ///   Register a ChainForwarder
        /// </summary>
        /// <param name = "forwarder"></param>
        public void AddForwarder(IChainForwarder forwarder)
        {
            _forwarders.Add(forwarder);
        }

        /// <summary>
        ///   Finds the matching BehaviorChain for the given IRouteDefinition.  If no
        ///   BehaviorChain exists, one is created and added to the BehaviorGraph
        /// </summary>
        /// <param name = "route"></param>
        /// <returns></returns>
        public BehaviorChain BehaviorFor(IRouteDefinition route)
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
        public BehaviorChain BehaviorFor<T>(Expression<Action<T>> expression)
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
        public BehaviorChain BehaviorFor<T>(Expression<Func<T, object>> expression)
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

        [Obsolete("Wanna make this go away in 2.0")]
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
        public BehaviorChain BehaviorFor(Type inputType)
        {
            var chains = Behaviors.Where(x => x.InputType() == inputType);
            if (chains.Count() == 1)
            {
                return chains.First();
            }

            if (chains.Count() == 0)
            {
                throw new FubuException(2150, "Could not find any behavior chains for input type {0}",
                    inputType.AssemblyQualifiedName);
            }

            throw new FubuException(2151, "Found more than one behavior chain for input type {0}",
                inputType.AssemblyQualifiedName);
        }

        /// <summary>
        /// Finds the BehaviorChain for an ActionCall
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public BehaviorChain BehaviorFor(ActionCall call)
        {
            return BehaviorForActionCall(call);
        }

        /// <summary>
        ///   Finds the Id of the single BehaviorChain
        ///   that matches the inputType
        /// </summary>
        /// <param name = "inputType"></param>
        /// <returns></returns>
        public Guid IdForType(Type inputType)
        {
            return BehaviorFor(inputType).UniqueId;
        }

        /// <summary>
        ///   Finds the Id of the BehaviorChain containing
        ///   the ActionCall
        /// </summary>
        public Guid IdForCall(ActionCall call)
        {
            return BehaviorForActionCall(call).UniqueId;
        }

        private BehaviorChain BehaviorForActionCall(ActionCall call)
        {
            var chain = Behaviors.FirstOrDefault(x => x.FirstCall().Equals(call));

            if (chain == null)
            {
                throw new FubuException(2152, "Could not find a behavior for action {0}", call.Description);
            }
            return chain;
        }

        /// <summary>
        ///   Finds all the BehaviorChains for the designated handler T
        /// </summary>
        public HandlerActionsSet ActionsForHandler<T>()
        {
            return ActionsForHandler(typeof (T));
        }

        /// <summary>
        ///   Finds all the BehaviorChain's for the designated handlerType
        /// </summary>
        public HandlerActionsSet ActionsForHandler(Type handlerType)
        {
            var actions = FirstActions().Where(x => x.HandlerType == handlerType);
            return new HandlerActionsSet(actions, handlerType);
        }

        /// <summary>
        ///   Finds HandlerActionSet's for all the handlers that match handlerFilter
        /// </summary>
        /// <param name = "handlerFilter"></param>
        /// <returns></returns>
        public IEnumerable<HandlerActionsSet> HandlerSetsFor(Func<Type, bool> handlerFilter)
        {
            return FirstActions()
                .Where(call => handlerFilter(call.HandlerType))
                .GroupBy(x => x.HandlerType)
                .Select(group => new HandlerActionsSet(group, group.Key));
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


    public class SortByRouteRankIterator : IRouteIterator
    {
        #region IRouteIterator Members

        public IEnumerable<RoutedChain> Over(IEnumerable<RoutedChain> behaviors)
        {
            return behaviors.OrderBy(b => b.Rank);
        }

        #endregion
    }


    public interface IRouteIterator
    {
        IEnumerable<RoutedChain> Over(IEnumerable<RoutedChain> behaviors);
    }
}