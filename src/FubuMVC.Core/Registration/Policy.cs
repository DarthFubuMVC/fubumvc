using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Core.Registration
{
    /// <summary>
    /// Class used to define BehaviorChain policies and conventions
    /// </summary>
    [CanBeMultiples]
    public class Policy : IConfigurationAction, DescribesItself
    {
        private readonly IList<IChainModification> _actions = new List<IChainModification>();
        private readonly IList<IChainFilter> _wheres = new List<IChainFilter>();

        /// <summary>
        /// Define the applicability of this policy
        /// </summary>
        public ChainPredicate Where
        {
            get
            {
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
            get { return new AddToEndExpression(this); }
        }

        /// <summary>
        /// Add additional "wrapping" behaviors to the beginning of a behavior chain
        /// </summary>
        public WrapWithExpression Wrap
        {
            get { return new WrapWithExpression(this); }
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
        public void ModifyBy(Action<BehaviorChain> alteration, string description = "User defined")
        {
            _actions.Add(new LambdaChainModification(alteration)
            {
                Description = description
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

        /// <summary>
        /// Apply content compression to matching chains
        /// </summary>
        public ContentCompressionActions ContentCompression
        {
            get { return new ContentCompressionActions(this); }
        }
    }
}