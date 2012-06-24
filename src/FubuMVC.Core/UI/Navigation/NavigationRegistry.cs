using System;
using System.Collections.Generic;
using FubuLocalization;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.UI.Navigation
{
    [CanBeMultiples]
    public class NavigationRegistry : IConfigurationAction
    {
        private readonly IList<Action<NavigationGraph>> _modifications = new List<Action<NavigationGraph>>();
        private StringToken _lastKey;

        // Dru can come back later and change this to suit his OCD while
        // I get stuff done now;-)
        private Action<NavigationGraph> modification
        {
            set
            {
                _modifications.Add(value);
            }
        }

        public void Configure(BehaviorGraph graph)
        {
            Configure(graph.Navigation);
            
        }

        internal void Configure(NavigationGraph graph)
        {
            _modifications.Each(x => x(graph));
        }

        public AddExpression ForMenu(string title)
        {
            return ForMenu(new NavigationKey(title));
        }

        public AddExpression ForMenu(StringToken key)
        {
            _lastKey = key;

            return new AddExpression(this);
        }

        public AddExpression Add
        {
            get
            {
                return new AddExpression(this);
            }
            set
            {
                // do nothing
            }
        }

        public class AddExpression
        {
            private readonly NavigationRegistry _parent;

            public AddExpression(NavigationRegistry parent)
            {
                _parent = parent;
            }

            public static AddExpression operator +(AddExpression original, MenuNode node)
            {
                original._parent.addToLastKey(node);

                return original;
            }
        }

        private void addToLastKey(MenuNode node)
        {
            var key = _lastKey;
            modification = graph =>
            {
                graph.AddNode(key, node);
            };
        }
    }
}