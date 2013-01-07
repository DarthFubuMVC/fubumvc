using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Registration.Conventions;
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
        public ChainPredicate Where
        {
            get { 
                var predicate = new ChainPredicate();
                _wheres.Add(predicate);

                return predicate;
            }
        }

        

        void IConfigurationAction.Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(chain => _wheres.All(x => x.Matches(chain)))
                .Each(chain => _actions.Each(x => x.Modify(chain)));
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

        /// <summary>
        /// Apply caching rules to matching chains
        /// </summary>
        public CachingExpression Caching
        {
            get
            {
                return new CachingExpression(this);
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