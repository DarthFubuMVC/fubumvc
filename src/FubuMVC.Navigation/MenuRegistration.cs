using System.Linq;
using FubuLocalization;

namespace FubuMVC.Navigation
{
    public class MenuRegistration : IMenuRegistration
    {
        private readonly IMenuPlacementStrategy _strategy;
        private readonly IStringTokenMatcher _matcher;
        private readonly MenuNode _node;

        public MenuRegistration(IMenuPlacementStrategy strategy, IStringTokenMatcher matcher, MenuNode node)
        {
            _strategy = strategy;
            _matcher = matcher;
            _node = node;
        }

        public bool DependsOn(StringToken token)
        {
            return _matcher.Matches(token);
        }

        public string Description
        {
            get { return _strategy.FormatDescription(_matcher.Description, _node.Key); }
        }

        public StringToken Key
        {
            get { return _node.Key; }
        }

        public IMenuPlacementStrategy Strategy
        {
            get { return _strategy; }
        }

        public void Configure(NavigationGraph graph)
        {
            var dependency = graph.AllNodes().FirstOrDefault(node => _matcher.Matches(node.Key));
            
            // code was like this for debugging
            if (dependency == null)
            {
                dependency = graph.MenuFor(_matcher.DefaultKey());
            }

            

            _strategy.Apply(dependency, _node);
        }

        public IStringTokenMatcher Matcher
        {
            get { return _matcher; }
        }

        public MenuNode Node
        {
            get { return _node; }
        }

        public override string ToString()
        {
            return Description;
        }
    }
}