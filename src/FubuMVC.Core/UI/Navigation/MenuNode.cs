using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuLocalization;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.UI.Navigation
{
    public class MenuNode : Node<MenuNode, MenuChain>
    {
        private readonly Func<IChainResolver, BehaviorChain> _finder;
        private readonly StringToken _key;
        private BehaviorChain _chain;
        private readonly MenuNodeType _type;
        private Type _isEnabledConditionType = typeof(Always);

        public MenuNode(StringToken key)
        {
            _key = key;
            _type = MenuNodeType.Node;

            Children = new MenuChain(key);
        }

        public MenuNode(StringToken key, Func<IChainResolver, BehaviorChain> finder)
            : this(key)
        {
            _type = MenuNodeType.Leaf;
            _finder = finder;
        }

        private MenuItemState _unauthorizedState = MenuItemState.Hidden;

        /// <summary>
        /// Sets what state this menu item would be if the authorization
        /// fails for the user.
        /// </summary>
        public MenuItemState UnauthorizedState
        {
            get { return _unauthorizedState ?? MenuItemState.Hidden; }
            set { _unauthorizedState = value; }
        }

        public StringToken Key
        {
            get { return _key; }
        }

        public MenuNodeType Type
        {
            get { return _type; }
        }

        public Type IsEnabledConditionType
        {
            get { return _isEnabledConditionType; }
            set
            {
                if (!value.CanBeCastTo<IConditional>())
                {
                    throw new ArgumentOutOfRangeException("Only types that implement IConditional may be used here");    
                }

                _isEnabledConditionType = value;
            }
        }

        public BehaviorChain BehaviorChain
        {
            get { return _chain; }
        }

        public void Resolve(IChainResolver resolver)
        {
            if (_finder == null) return;

            _chain = _finder(resolver);

            if (_chain == null)
            {
                throw new InvalidOperationException("Unable to find the requested behavior chain for menu " + Key);
            }
        }

        public object UrlInput { get; set; }

        public MenuChain Children { get; private set; }

        public IEnumerable<MenuNode> FindAllChildren()
        {
            foreach (var node in Children)
            {
                yield return node;

                foreach (var child in node.FindAllChildren())
                {
                    yield return child;
                }
            }
        }

        public static MenuNode ForChain(StringToken key, BehaviorChain chain)
        {
            return new MenuNode(key, r => chain){
                _chain = chain
            };
        }

        public static MenuNode ForInput<T>(StringToken key, T input = null) where T : class
        {
            return new MenuNode(key, r => r.FindUniqueByType(typeof(T), category:"GET")){
                UrlInput = input
            };
        }

        public static MenuNode ForAction<T>(StringToken key, Expression<Action<T>> method)
        {
            return new MenuNode(key, r => r.Find(method, category:"GET"));
        }

        public static MenuNode ForCreatorOf<T>(StringToken key)
        {
            var type = typeof(T);
            return ForCreatorOf(key, type);
        }

        public static MenuNode ForCreatorOf(StringToken key, Type type)
        {
            return new MenuNode(key, r => r.FindCreatorOf(type));
        }

        public string CreateUrl()
        {
            return _chain.Route.CreateUrlFromInput(UrlInput);
        }

        public override string ToString()
        {
            return string.Format("MenuNode: {0}", _key);
        }
    }
}