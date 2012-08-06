using System;
using System.Collections.Generic;
using FubuLocalization;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.UI.Navigation;
using System.Linq;

namespace FubuMVC.Core.Registration.Conventions
{   
    [ConfigurationType(ConfigurationType.ByNavigation)]
    public class NavigationRootPolicy : IConfigurationAction
    {
        private readonly IList<NavigationRootModification> _modifications = new List<NavigationRootModification>();

        public void ForKey(string key)
        {
            ForKey(new NavigationKey(key));
        }

        public void ForKey(StringToken key)
        {
            var modifier = new NavigationRootModification(graph => graph.FindNode(key));
            _modifications.Add(modifier);
        }

        public void RequireRole(string role)
        {
            Alter(chain => chain.Authorization.AddRole(role));
        }

        public void Alter(Action<BehaviorChain> alteration)
        {
            _modifications.Last().Alter(alteration);
        }

        public void Configure(BehaviorGraph graph)
        {
            _modifications.Each(x => x.Apply(graph.Navigation));
        }

        public void WrapWithChrome<TChrome>() where TChrome : ChromeContent
        {
            Alter(chain =>
            {
                var outputNode = chain.OfType<OutputNode>().SingleOrDefault();
                if (outputNode != null)
                {
                    outputNode.AddBefore(new ChromeNode(typeof(TChrome)));
                }
            });
        }
    }

    public class NavigationRootModification
    {
        private readonly Func<NavigationGraph, IMenuNode> _finder;
        private readonly IList<Action<BehaviorChain>> _alterations = new List<Action<BehaviorChain>>();

        public NavigationRootModification(Func<NavigationGraph, IMenuNode> finder)
        {
            _finder = finder;
        }

        public void Alter(Action<BehaviorChain> alterations)
        {
            _alterations.Add(alterations);
        }

        public void Apply(NavigationGraph graph)
        {
            var node = _finder(graph);
            node.AllChains().Each(chain => _alterations.Each(x => x(chain)));
        }
    }
}