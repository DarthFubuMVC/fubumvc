using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration
{
    public class BehaviorGraph
    {
        public BehaviorGraph(IConfigurationObserver observer)
        {
            RouteIterator = new SortByRouteRankIterator(); // can override in a registry
            Observer = observer;
        }

        private readonly List<BehaviorChain> _behaviors = new List<BehaviorChain>();
        private readonly IServiceRegistry _services = new ServiceRegistry();

        public IConfigurationObserver Observer { get; private set; }

        public IServiceRegistry Services { get { return _services; } }

        public IEnumerable<IRouteDefinition> Routes { get { return _behaviors.Select(x => x.Route).Where(x => x != null); } }


        public int BehaviorChainCount { get { return _behaviors.Count; } }

        public IEnumerable<BehaviorChain> Behaviors { get { return _behaviors; } }

        public IRouteIterator RouteIterator { get; set; }


        public void RegisterRoute(BehaviorChain chain, IRouteDefinition route)
        {
            _behaviors.Fill(chain);
            chain.Route = route;
        }

        public BehaviorChain BehaviorFor(IRouteDefinition route)
        {
            BehaviorChain chain = _behaviors.FirstOrDefault(x => x.Route == route);
            if (chain == null)
            {
                chain = new BehaviorChain
                {
                    Route = route
                };
                _behaviors.Fill(chain);
            }

            return chain;
        }

        public IEnumerable<RouteDefinition<T>> RoutesFor<T>()
        {
            return _behaviors.Select(x => x.Route).Where(x => x != null).OfType<RouteDefinition<T>>();
        }

        public void EachService(Action<Type, ObjectDef> action)
        {
            /*
             * 1.) Loop through each service
             * 2.) Loop through each top level behavior for routes
             * 3.) Loop through each partial behavior
             * 4.) add in the UrlRegistry as a value
             */

            _services.Each(action);

            _behaviors.Each(chain => { action(typeof (IActionBehavior), chain.ToObjectDef()); });

            action(typeof (BehaviorGraph), new ObjectDef
            {
                Value = this
            });
        }

        public IEnumerable<ActionCall> Actions()
        {
            return allActions().ToList();
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

        public IRouteDefinition RouteFor<T>(Expression<Action<T>> expression)
        {
            BehaviorChain chain = BehaviorFor(expression);
            return chain == null ? null : chain.Route;
        }

        public BehaviorChain BehaviorFor<T>(Expression<Action<T>> expression)
        {
            ActionCall call = ActionCall.For(expression);
            return _behaviors.Where(x => x.Calls.Contains(call)).FirstOrDefault();
        }

        public BehaviorChain BehaviorFor<T>(Expression<Func<T, object>> expression)
        {
            ActionCall call = ActionCall.For(expression);
            return _behaviors.Where(x => x.Calls.Contains(call)).FirstOrDefault();
        }

        public void VisitRoutes(IRouteVisitor visitor)
        {
            RouteIterator.Over(_behaviors).Each(x => visitor.VisitRoute(x.Route, x));
        }

        public void Describe()
        {
            _behaviors.Each(x => { Debug.WriteLine(x.FirstCall().Description.PadRight(70) + x.Route.Pattern); });
        }

        public void VisitRoutes(Action<RouteVisitor> configure)
        {
            var visitor = new RouteVisitor();
            configure(visitor);
            VisitRoutes(visitor);
        }

        public void VisitBehaviors(IBehaviorVisitor visitor)
        {
            _behaviors.Each(visitor.VisitBehavior);
        }
        
        public void AddChain(BehaviorChain chain)
        {
            _behaviors.Add(chain);
        }

        public void Import(BehaviorGraph graph, string prefix)
        {
            graph.Behaviors.Each(b =>
            {
                AddChain(b);
                b.PrependToUrl(prefix);
            });
        }

        public Guid IdForType(Type inputType)
        {
            IEnumerable<BehaviorChain> chains = Behaviors.Where(x => x.ActionInputType() == inputType);
            if (chains.Count() == 1)
            {
                return chains.First().UniqueId;
            }

            if (chains.Count() == 0)
            {
                throw new FubuException(2150, "Could not find any behavior chains for input type {0}",
                                        inputType.AssemblyQualifiedName);
            }

            throw new FubuException(2151, "Found more than one behavior chain for input type {0}",
                                    inputType.AssemblyQualifiedName);
        }

        public Guid IdForCall(ActionCall call)
        {
            BehaviorChain chain = Behaviors.FirstOrDefault(x => x.FirstCall().Equals(call));

            if (chain == null)
            {
                throw new FubuException(2152, "Could not find a behavior for action {0}", call.Description);
            }

            return chain.UniqueId;
        }
    }

    public class SortByRouteRankIterator : IRouteIterator
    {
        public IEnumerable<BehaviorChain> Over(IEnumerable<BehaviorChain> behaviors)
        {
            return behaviors.OrderBy(b => b.Route.Rank);
        }
    }

    public interface IRouteIterator
    {
        IEnumerable<BehaviorChain> Over(IEnumerable<BehaviorChain> behaviors);
    }
}