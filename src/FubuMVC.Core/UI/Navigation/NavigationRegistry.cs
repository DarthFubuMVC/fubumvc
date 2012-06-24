using System;
using System.Collections.Generic;
using FubuLocalization;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.UI.Navigation
{
    [CanBeMultiples]
    public class NavigationRegistry : IConfigurationAction
    {
        private readonly IList<IMenuRegistration> _registrations = new List<IMenuRegistration>();
        private StringToken _lastKey;

        public void Configure(BehaviorGraph graph)
        {
            Configure(graph.Navigation);
            
        }

        internal void Configure(NavigationGraph graph)
        {
            graph.AddRegistrations(_registrations);
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
            _registrations.Add(new MenuRegistration(new AddChild(), new Literal(key), node));
        }
    }
}