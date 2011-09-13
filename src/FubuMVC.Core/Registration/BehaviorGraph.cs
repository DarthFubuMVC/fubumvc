using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration
{
    public interface IRegisterable
    {
        void Register(Action<Type, ObjectDef> action);
    }

    public interface IChainImporter
    {
        void Import(BehaviorGraph graph, Action<BehaviorChain> alternation);
    }

    public class BehaviorGraph : IRegisterable, IChainImporter
    {
        private readonly List<BehaviorChain> _behaviors = new List<BehaviorChain>();
        private readonly Cache<Type, ChainGroup> _chainsForMethod;

        private readonly Cache<Type, IEnumerable<BehaviorChain>> _chainsForType =
            new Cache<Type, IEnumerable<BehaviorChain>>();

        private readonly Cache<Type, Cache<string, IEnumerable<BehaviorChain>>> _chainsForTypeAndCategory =
            new Cache<Type, Cache<string, IEnumerable<BehaviorChain>>>();

        private readonly Cache<Type, BehaviorChain> _creators;

        private readonly List<IChainForwarder> _forwarders = new List<IChainForwarder>();
        private readonly IServiceRegistry _services = new ServiceRegistry();

        public BehaviorGraph(IConfigurationObserver observer)
        {
            RouteIterator = new SortByRouteRankIterator(); // can override in a registry
            Observer = observer;

            _chainsForType.OnMissing = findChainsByType;

            _chainsForTypeAndCategory.OnMissing =
                type =>
                new Cache<string, IEnumerable<BehaviorChain>>(
                    category => { return ChainsFor(type).Where(x => x.UrlCategory.Category == category).ToList(); });

            _chainsForMethod = new Cache<Type, ChainGroup>(type => new ChainGroup(type, this));

            _creators =
                new Cache<Type, BehaviorChain>(
                    type => { return Behaviors.SingleOrDefault(x => x.UrlCategory.Creates.Contains(type)); });
        }

        public BehaviorGraph() : this(new NulloConfigurationObserver())
        {
        }

        public IEnumerable<IChainForwarder> Forwarders
        {
            get { return _forwarders; }
        }

        public IConfigurationObserver Observer { get; private set; }

        public IServiceRegistry Services
        {
            get { return _services; }
        }

        public IEnumerable<IRouteDefinition> Routes
        {
            get { return _behaviors.Select(x => x.Route).Where(x => x != null); }
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

        void IChainImporter.Import(BehaviorGraph graph, Action<BehaviorChain> alternation)
        {
            graph.Behaviors.Each(b =>
            {
                AddChain(b);
                alternation(b);
            });
        }

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

            action(typeof (BehaviorGraph), new ObjectDef{
                Value = this
            });
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
            var chain = _behaviors.FirstOrDefault(x => x.Route == route);
            if (chain == null)
            {
                chain = new BehaviorChain{
                    Route = route
                };
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
            foreach (BehaviorChain chain in _behaviors)
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
            foreach (BehaviorChain chain in _behaviors)
            {
                foreach (ActionCall call in chain.Calls)
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
            return _behaviors.Where(x => x.Calls.Contains(call)).FirstOrDefault();
        }

        /// <summary>
        ///   Applies the giving IRouteVisitor to each IRouteDefinition
        /// </summary>
        /// <param name = "visitor"></param>
        public void VisitRoutes(IRouteVisitor visitor)
        {
            RouteIterator.Over(_behaviors).Each(x => visitor.VisitRoute(x.Route, x));
        }

        /// <summary>
        ///   Writes a summary of the BehaviorGraph to Trace
        /// </summary>
        public void Describe()
        {
            _behaviors.Each(x => { Trace.WriteLine(x.FirstCall().Description.PadRight(70) + x.Route.Pattern); });
        }

        /// <summary>
        ///   Applies the giving IRouteVisitor to each IRouteDefinition
        /// </summary>
        /// <param name = "configure"></param>
        public void VisitRoutes(Action<RouteVisitor> configure)
        {
            var visitor = new RouteVisitor();
            configure(visitor);
            VisitRoutes(visitor);
        }

        /// <summary>
        ///   Applies the given IBehaviorVisitor to each BehaviorChain
        /// </summary>
        /// <param name = "visitor"></param>
        public void VisitBehaviors(IBehaviorVisitor visitor)
        {
            _behaviors.Each(visitor.VisitBehavior);
        }

        public void AddChain(BehaviorChain chain)
        {
            _behaviors.Add(chain);
        }

        public void RemoveChain(BehaviorChain chain)
        {
            _behaviors.Remove(chain);
            _chainsForType.ClearAll();
            _chainsForTypeAndCategory.ClearAll();
        }

        /// <summary>
        ///   Adds a BehaviorChain for the given url pattern and action type.
        ///   Specify the "arguments" parameters if actionType is an open
        ///   generic type
        /// </summary>
        /// <param name = "urlPattern"></param>
        /// <param name = "actionType"></param>
        /// <param name = "arguments"></param>
        /// <returns></returns>
        public BehaviorChain AddActionFor(string urlPattern, Type actionType, params Type[] arguments)
        {
            if (arguments.Any())
            {
                var closedType = actionType.MakeGenericType(arguments);
                return AddActionFor(urlPattern, closedType);
            }

            var action = ActionCall.For(actionType);
            var chain = new BehaviorChain();
            chain.AddToEnd(action);
            chain.Route = action.BuildRouteForPattern(urlPattern);
            AddChain(chain);

            return chain;
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
        /// <param name = "call"></param>
        /// <returns></returns>
        public Guid IdForCall(ActionCall call)
        {
            var chain = Behaviors.FirstOrDefault(x => x.FirstCall().Equals(call));

            if (chain == null)
            {
                throw new FubuException(2152, "Could not find a behavior for action {0}", call.Description);
            }

            return chain.UniqueId;
        }

        /// <summary>
        ///   Finds all the BehaviorChain's for the designated handler T
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <returns></returns>
        public HandlerActionsSet ActionsForHandler<T>()
        {
            return ActionsForHandler(typeof (T));
        }

        /// <summary>
        ///   Finds all the BehaviorChain's for the designated handlerType
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <returns></returns>
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

        private IEnumerable<BehaviorChain> findChainsByType(Type modelType)
        {
            return Behaviors.Where(x => x.InputType() == modelType).ToList();
        }

        public IEnumerable<BehaviorChain> ChainsFor(Type modelType)
        {
            return _chainsForType[modelType];
        }

        public IEnumerable<BehaviorChain> ChainsFor(Type modelType, string category)
        {
            return _chainsForTypeAndCategory[modelType][category];
        }

        public IEnumerable<BehaviorChain> ChainsFor(Type handlerType, MethodInfo method)
        {
            return _chainsForMethod[handlerType].ChainsFor(method);
        }

        public BehaviorChain ChainThatCreates(Type type)
        {
            return _creators[type];
        }

        public BehaviorChain FindHomeChain()
        {
            return Behaviors.FirstOrDefault(x => x.Route != null && x.Route.Pattern == string.Empty);
        }
    }

    public class ChainGroup
    {
        private readonly Cache<string, IEnumerable<BehaviorChain>> _chains;

        public ChainGroup(Type handlerType, BehaviorGraph graph)
        {
            _chains = new Cache<string, IEnumerable<BehaviorChain>>(method =>
            {
                return graph.Behaviors.Where(x => x.FirstCall() != null).Where(x =>
                {
                    var call = x.FirstCall();
                    return call.HandlerType == handlerType && call.Method.Name == method;
                }).ToList();
            });
        }

        public IEnumerable<BehaviorChain> ChainsFor(MethodInfo method)
        {
            return _chains[method.Name];
        }
    }

    public class SortByRouteRankIterator : IRouteIterator
    {
        public IEnumerable<BehaviorChain> Over(IEnumerable<BehaviorChain> behaviors)
        {
            return behaviors.OrderBy(b => b.Rank);
        }
    }

    public interface IRouteIterator
    {
        IEnumerable<BehaviorChain> Over(IEnumerable<BehaviorChain> behaviors);
    }
}