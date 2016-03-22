using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.DependencyAnalysis;
using FubuCore.Util;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.Navigation
{
    
    public interface IMenuResolver
    {
        MenuChain MenuFor(StringToken key);
    }

    public class MenuResolverCache : IMenuResolver
    {
        private readonly Lazy<NavigationGraph> _inner; 

        public MenuResolverCache(IChainResolver resolver, NavigationGraph graph)
        {
            _inner = new Lazy<NavigationGraph>(() => {
                graph.Compile();
                graph.AllNodes().OfType<MenuNode>().Each(x =>
                {
                    try
                    {
                        x.Resolve(resolver);
                    }
                    catch (Exception ex)
                    {
                        throw new FubuException(4001, ex, "Failed to resolve a BehaviorChain for navigation element " + x.Key);
                    }

                });

                return graph;
            });
        }

        public MenuChain MenuFor(StringToken key)
        {
            return _inner.Value.MenuFor(key);
        }
    }

    public class NavigationGraph
    {
        private readonly Cache<StringToken, MenuChain> _chains = new Cache<StringToken, MenuChain>(key => new MenuChain(key));

        private readonly IList<IMenuRegistration> _registrations = new List<IMenuRegistration>();

        public NavigationGraph()
        {
        }

        public NavigationGraph(Action<NavigationRegistry> configure)
        {
            var registry = new NavigationRegistry();
            configure(registry);

            registry.Configure(this);
        }

        public void AddRegistration(IMenuRegistration registration)
        {
            _registrations.Add(registration);
        }

        public void Compile()
        {
            Func<IMenuRegistration, IEnumerable<string>> dependencyFinder = r =>
            {
                return _registrations.Where(x => r.DependsOn(x.Key)).Select(x => x.Description);
            };

            var graph = new DependencyGraph<IMenuRegistration>(r => r.Description, dependencyFinder);
            _registrations.Each(graph.RegisterItem);
            
            graph.Ordered().Each(x =>
            {
                x.Configure(this);
            });

            _registrations.Clear();
        }

        public IMenuNode FindNode(StringToken key)
        {
            return AllNodes().FirstOrDefault(x => x.Key.Equals(key));
        }

        public MenuChain MenuFor(StringToken key)
        {
            return _chains[key];
        }

        public IEnumerable<IMenuNode> AllNodes()
        {
            foreach (var chain in _chains)
            {
                yield return chain;

                foreach (var node in chain.AllNodes())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<MenuChain> AllMenus()
        {
            return _chains;
        }

        public MenuChain MenuFor(string key)
        {
            return MenuFor(new NavigationKey(key));
        }

        public void AddRegistrations(IEnumerable<IMenuRegistration> registrations)
        {
            _registrations.AddRange(registrations);
        }

        public void AddChildNode(StringToken parent, MenuNode node)
        {
            var registration = new MenuRegistration(new AddChild(), new Literal(parent), node);
            _registrations.Add(registration);
        }
    }
}