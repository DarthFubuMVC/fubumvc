using System;
using System.Collections.Generic;
using FubuLocalization;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using System.Linq;

namespace FubuMVC.Navigation
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
            Alter(chain => chain.BehaviorChain.Authorization.AddRole(role));
        }

        public void Alter(Action<MenuNode> alteration)
        {
            _modifications.Last().Alter(alteration);
        }

        public void Configure(BehaviorGraph graph)
        {
            _modifications.Each(x => x.Apply(graph.Settings.Get<NavigationGraph>()));
        }

        public void WrapWithChrome<TChrome>() where TChrome : ChromeContent
        {
            Alter(node =>
            {
                var outputNode = node.BehaviorChain.OfType<OutputNode>().SingleOrDefault();
                if (outputNode != null)
                {
                    var chromeNode = new ChromeNode(typeof(TChrome));
                    chromeNode.Title = () => node.Key.ToString();

                    outputNode.AddBefore(chromeNode);
                }
            });
        }
    }

    public class NavigationRootModification
    {
        private readonly Func<NavigationGraph, IMenuNode> _finder;
        private readonly IList<Action<MenuNode>> _alterations = new List<Action<MenuNode>>();

        public NavigationRootModification(Func<NavigationGraph, IMenuNode> finder)
        {
            _finder = finder;
        }

        public void Alter(Action<MenuNode> alterations)
        {
            _alterations.Add(alterations);
        }

        public void Apply(NavigationGraph graph)
        {
            var node = _finder(graph);
            node.AllNodes().Where(x => x.BehaviorChain != null).Each(n => _alterations.Each(x => x(n)));
        }
    }
}