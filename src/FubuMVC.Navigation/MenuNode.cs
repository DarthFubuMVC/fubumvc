using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuLocalization;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime.Conditionals;
using FubuCore.Reflection;
using FubuCore;

namespace FubuMVC.Navigation
{
    public interface IMenuNode : INode<MenuNode>
    {
        StringToken Key { get; }
        void AddChild(MenuNode node);

        IEnumerable<BehaviorChain> AllChains();
        IEnumerable<MenuNode> AllNodes();
    }

    public class MenuNode : Node<MenuNode, MenuChain>, IMenuNode
    {
        private readonly Func<IChainResolver, BehaviorChain> _finder;
        private readonly StringToken _key;
        private readonly MenuNodeType _type;
        private BehaviorChain _chain;
        private Type _hideConditionalType = typeof (Never);
        private string _icon;
        private Type _isEnabledConditionType = typeof (Always);
        private MenuItemState _unauthorizedState = MenuItemState.Hidden;

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

        /// <summary>
        ///   Sets what state this menu item would be if the authorization
        ///   fails for the user.
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

        public void AddChild(MenuNode node)
        {
            Children.AddToEnd(node);
        }

        public IEnumerable<BehaviorChain> AllChains()
        {
            if (_chain != null)
            {
                yield return _chain;
            }

            foreach (var child in Children)
            {
                if (child.BehaviorChain != null)
                {
                    yield return child.BehaviorChain;
                }
            }
        }

        public IEnumerable<MenuNode> AllNodes()
        {
            yield return this;

            foreach (var child in Children)
            {
                yield return child;
            }
        }

        public MenuNodeType Type
        {
            get { return _type; }
        }

        public Type HideIfConditional
        {
            get { return _hideConditionalType; }
        }

        public BehaviorChain BehaviorChain
        {
            get { return _chain; }
        }

        public object UrlInput { get; set; }

        public MenuChain Children { get; private set; }

        public MenuNode IsEnabledBy<T>() where T : IConditional
        {
            return IsEnabledBy(typeof (T));
        }


        public MenuNode IsEnabledBy(Type value)
        {
            if (!value.CanBeCastTo<IConditional>())
            {
                throw new ArgumentOutOfRangeException("Only types that implement IConditional may be used here");
            }

            _isEnabledConditionType = value;

            return this;
        }

        public MenuNode HideIf<T>() where T : IConditional
        {
            return HideIf(typeof (T));
        }

        public MenuNode HideIf(Type conditionalType)
        {
            if (!conditionalType.CanBeCastTo<IConditional>())
            {
                throw new ArgumentOutOfRangeException("Only types that implement IConditional may be used here");
            }

            _hideConditionalType = conditionalType;

            return this;
        }


        public Type IsEnabledBy()
        {
            return _isEnabledConditionType;
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

        public static MenuNode ForChain(string key, BehaviorChain chain)
        {
            return ForChain(new NavigationKey(key), chain);
        }

        public static MenuNode ForChain(StringToken key, BehaviorChain chain)
        {
            return new MenuNode(key, r => chain){
                _chain = chain
            };
        }

        public static MenuNode ForInput<T>(StringToken key, T input = null) where T : class
        {
            return new MenuNode(key, r => r.FindUniqueByType(typeof (T), category: "GET")){
                UrlInput = input
            };
        }

        public static MenuNode ForInput<T>(string title, T input = null) where T : class
        {
            return ForInput<T>(new NavigationKey(title), input: input);
        }

        public static MenuNode ForAction<T>(StringToken key, Expression<Action<T>> method)
        {
            return new MenuNode(key, r => r.Find(method, category: "GET"));
        }

        public static MenuNode ForAction<T>(string title, Expression<Action<T>> method)
        {
            return ForAction(new NavigationKey(title), method);
        }

        public static MenuNode ForCreatorOf<T>(StringToken key)
        {
            var type = typeof (T);
            return ForCreatorOf(key, type);
        }

        public static MenuNode ForCreatorOf(StringToken key, Type type)
        {
            return new MenuNode(key, r => r.FindCreatorOf(type));
        }

        public static MenuNode ForCreatorOf<T>(string key)
        {
            return ForCreatorOf<T>(new NavigationKey(key));
        }

        public static MenuNode ForCreatorOf(string key, Type type)
        {
            return ForCreatorOf(new NavigationKey(key), type);
        }

        public static MenuNode Node(StringToken key)
        {
            return new MenuNode(key);
        }

        public static MenuNode Node(string key)
        {
            return Node(new NavigationKey(key));
        }

        public string CreateUrl()
        {
            return _chain.Route.CreateUrlFromInput(UrlInput);
        }

        public override string ToString()
        {
            return string.Format("MenuNode: {0}", _key);
        }

        public string Icon()
        {
            return _icon;
        }

        public MenuNode Icon(string icon)
        {
            _icon = icon;
            return this;
        }


    }
}