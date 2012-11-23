using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration
{
    /// <summary>
    /// Class used to define BehaviorChain policies and conventions
    /// </summary>
    [CanBeMultiples]
    public class Policy : IConfigurationAction, DescribesItself, IKnowMyConfigurationType
    {
        private readonly IList<IChainModification> _actions = new List<IChainModification>();
        private readonly IList<IChainFilter> _wheres = new List<IChainFilter>();

        /// <summary>
        /// Define the applicability of this policy
        /// </summary>
        public WhereExpression Where
        {
            get { return new WhereExpression(this, filter => _wheres.Add(filter)); }
        }

        

        void IConfigurationAction.Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(chain => _wheres.All(x => x.Matches(chain)))
                .Each(chain => _actions.Each(x => x.Modify(chain)));
        }

        private void registerOrFilter(IChainFilter filter)
        {
            IChainFilter last = _wheres.LastOrDefault();
            if (last == null)
            {
                _wheres.Add(filter);
            }
            else
            {
                if (last is OrChainFilter)
                {
                    last.As<OrChainFilter>().Add(filter);
                }
                else
                {
                    _wheres.Remove(last);
                    var orFilter = new OrChainFilter();
                    orFilter.Add(last);
                    orFilter.Add(filter);

                    _wheres.Add(orFilter);
                }
            }
        }

        public interface IOrExpression
        {
            WhereExpression Or { get; }
        }

        /// <summary>
        /// Add additional nodes to the end of the BehaviorChain
        /// </summary>
        public AddToEndExpression Add
        {
            get
            {
                return new AddToEndExpression(this);
            }
        }

        /// <summary>
        /// Add additional "wrapping" behaviors to the beginning of a behavior chain
        /// </summary>
        public WrapWithExpression Wrap
        {
            get
            {
                return new WrapWithExpression(this);
            }
        }

        public class WhereExpression : IOrExpression
        {
            private readonly Policy _parent;
            private readonly Action<IChainFilter> _register;

            public WhereExpression(Policy parent, Action<IChainFilter> register)
            {
                _register = register;
                _parent = parent;
            }

            /// <summary>
            /// Directly add an IChainFilter "where" filter to this policy
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public IOrExpression Matching<T>() where T : IChainFilter, new()
            {
                return addFilter(new T());
            }


            /// <summary>
            /// Configure the applicability of this policy by matching against the *last* ActionCall
            /// in the chain
            /// </summary>
            /// <param name="filter"></param>
            /// <param name="description"></param>
            /// <returns></returns>
            public IOrExpression LastActionMatches(Func<ActionCall, bool> filter, string description)
            {
                return addFilter(new LastActionMatch(filter, description));
            }

            /// <summary>
            /// Configure the applicability of this policy by matching against the *last* ActionCall
            /// in the chain
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public IOrExpression LastActionMatches(Expression<Func<ActionCall, bool>> expression)
            {
                return addFilter(new LastActionMatch(expression));
            }

            /// <summary>
            /// Configure the applicability of this policy by matching against any ActionCall
            /// in the chain
            /// </summary>
            /// <param name="filter"></param>
            /// <param name="description">Optional diagnostic description of the 'where' filter</param>
            /// <returns></returns>
            public IOrExpression AnyActionMatches(Func<ActionCall, bool> filter, string description = "User defined")
            {
                return addFilter(new AnyActionMatch(filter, description));
            }

            /// <summary>
            /// Configure the applicability of this policy by matching against any ActionCall
            /// in the chain
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public IOrExpression AnyActionMatches(Expression<Func<ActionCall, bool>> expression)
            {
                return addFilter(new AnyActionMatch(expression));
            } 

            /// <summary>
            /// This policy will only apply to chains that are marked as IsPartialOnly
            /// </summary>
            /// <returns></returns>
            public IOrExpression IsPartialOnly()
            {
                return addFilter(new IsPartial());
            }

            /// <summary>
            /// This policy will only apply to chains that are not marked as IsPartialOnly
            /// </summary>
            /// <returns></returns>
            public IOrExpression IsNotPartial()
            {
                return addFilter(new IsNotPartial());
            }

            /// <summary>
            /// Limit the policy to chains where the resource (output) type can be cast to "T"
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public IOrExpression ResourceTypeImplements<T>()
            {
                return addFilter(new ResourceTypeImplements<T>());
            }

            /// <summary>
            /// Limit the policy to chains where the resource (output) type is T
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public IOrExpression ResourceTypeIs<T>()
            {
                return addFilter(new ResourceTypeIs<T>());
            }

            /// <summary>
            /// Limit the policy to chains where the input type can be cast to T
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public IOrExpression InputTypeImplements<T>()
            {
                return addFilter(new InputTypeImplements<T>());
            }

            /// <summary>
            /// Limit the policy to chains where the input type is T
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public IOrExpression InputTypeIs<T>()
            {
                return addFilter(new InputTypeIs<T>());
            }

            /// <summary>
            /// Use your own filter to limit the applicability of this policy
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public IOrExpression ChainMatches(Expression<Func<BehaviorChain, bool>> expression)
            {
                return addFilter(new LambdaChainFilter(expression));
            }

            /// <summary>
            /// Use your own filter to limit the applicability of this policy
            /// </summary>
            /// <param name="filter"></param>
            /// <param name="description">Optional description used in diagnostics</param>
            /// <returns></returns>
            public IOrExpression ChainMatches(Func<BehaviorChain, bool> filter, string description = "User defined")
            {
                return addFilter(new LambdaChainFilter(filter, description));
            }

            private IOrExpression addFilter(IChainFilter filter)
            {
                _register(filter);
                return this;
            }

            /// <summary>
            /// Chain multiple filters together in a boolean 'Or' manner
            /// </summary>
            public WhereExpression Or
            {
                get { return new WhereExpression(_parent, _parent.registerOrFilter); }
            }
        }

        /// <summary>
        /// Directly adds an IChainModification to the policy as an action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ModifyWith<T>() where T : IChainModification, new()
        {
            _actions.Add(new T());
        }

        /// <summary>
        /// Configure content negotiation and media readers and writers for behavior chains matching this policy
        /// </summary>
        public ConnegExpression Conneg
        {
            get
            {
                return new ConnegExpression(this);
            }
        }

        /// <summary>
        /// Add a modification to a BehaviorChain
        /// </summary>
        /// <param name="chainModification"></param>
        public void ModifyWith(IChainModification chainModification)
        {
            _actions.Add(chainModification);
        }

        /// <summary>
        /// Add a modification to behavior chains matching this policy
        /// </summary>
        /// <param name="alteration"></param>
        /// <param name="description">Optional description for diagnostics</param>
        public void ModifyBy(Action<BehaviorChain> alteration, string description = "User defined", string configurationType = ConfigurationType.Explicit)
        {
            _actions.Add(new LambdaChainModification(alteration)
            {
                Description = description,
                ConfigurationType = configurationType
            });
        }

        void DescribesItself.Describe(Description description)
        {
            describe(description);
        }

        protected virtual void describe(Description description)
        {
            description.AddList("Where", _wheres);
            description.AddList("Actions", _actions);
        }

        string IKnowMyConfigurationType.DetermineConfigurationType()
        {
            if (GetType().HasAttribute<ConfigurationTypeAttribute>())
            {
                return GetType().GetAttribute<ConfigurationTypeAttribute>().Type;
            }

            var types = _actions.Select(DetermineConfigurationType).Distinct().ToArray();

            if (types.Count() == 1)
            {
                return types.Single();
            }


            foreach (var type in BehaviorGraphBuilder.ConfigurationOrder().Reverse())
            {
                if (types.Contains(type)) return type;
            }

            return ConfigurationType.Policy;

        }

        /// <summary>
        /// Apply content compression to matching chains
        /// </summary>
        public ContentCompressionActions ContentCompression
        {
            get
            {
                return new ContentCompressionActions(this);
            }
        }

        public static string DetermineConfigurationType(IChainModification modification)
        {
            string configurationType = null;

            modification.GetType().ForAttribute<ConfigurationTypeAttribute>(att => configurationType = att.Type);

            var lambda = modification as LambdaChainModification;
            if (lambda != null)
            {
                configurationType = lambda.ConfigurationType;
            }

            return configurationType;
        }
    }


}