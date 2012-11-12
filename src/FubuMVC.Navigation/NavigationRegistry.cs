using System.Collections.Generic;
using FubuLocalization;
using FubuMVC.Core;
using FubuMVC.Core.Registration;

namespace FubuMVC.Navigation
{
    [CanBeMultiples, ConfigurationType(ConfigurationType.Settings)]
    public class NavigationRegistry : IConfigurationAction
    {
        private readonly IList<IMenuRegistration> _registrations = new List<IMenuRegistration>();
        private StringToken _lastKey;

        public void Configure(BehaviorGraph graph)
        {
            graph.Settings.Alter<NavigationGraph>(Configure);
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

        public InsertAfterExpression InsertAfter
        {
            get
            {
                return new InsertAfterExpression(this);
            }
        }

        public InsertBeforeExpression InsertBefore
        {
            get
            {
                return new InsertBeforeExpression(this);
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

        public class InsertBeforeExpression
        {
            private readonly NavigationRegistry _registry;

            public InsertBeforeExpression(NavigationRegistry registry)
            {
                _registry = registry;
            }

            public MenuNode this[StringToken parentKey]
            {
                set
                {
                    var registration = new MenuRegistration(new AddBefore(), new Literal(parentKey), value);
                    _registry._registrations.Add(registration);
                }
            }

            public MenuNode this[string parentKey]
            {
                set
                {
                    var registration = new MenuRegistration(new AddBefore(), new ByName(parentKey), value);
                    _registry._registrations.Add(registration);
                }
            }
        }

        public class InsertAfterExpression
        {
            private readonly NavigationRegistry _registry;

            public InsertAfterExpression(NavigationRegistry registry)
            {
                _registry = registry;
            }

            public MenuNode this[StringToken parentKey]
            {
                set
                {
                    var registration = new MenuRegistration(new AddAfter(), new Literal(parentKey), value);
                    _registry._registrations.Add(registration);
                }
            }

            public MenuNode this[string parentKey]
            {
                set
                {
                    var registration = new MenuRegistration(new AddAfter(), new ByName(parentKey), value);
                    _registry._registrations.Add(registration);
                }
            }
        }

        private void addToLastKey(MenuNode node)
        {
            var key = _lastKey;
            _registrations.Add(new MenuRegistration(new AddChild(), new Literal(key), node));
        }
    }
}