using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class ChainPredicate : IChainFilter, DescribesItself, IOrExpression
    {
        private readonly IList<IChainFilter> _filters = new List<IChainFilter>();
        private Action<IChainFilter> _add;

        public ChainPredicate()
        {
            _add = f => _filters.Add(f);
        }

        bool IChainFilter.Matches(BehaviorChain chain)
        {
            return _filters.Any(x => x.Matches(chain));
        }

        void DescribesItself.Describe(Description description)
        {
            description.Title = "Matches any of these filters";
            description.AddList("Filters", _filters);
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
        /// Directly add an IChainFilter "where" filter to this policy
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IOrExpression Matching(IChainFilter filter)
        {
            return addFilter(filter);
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

        

        private IOrExpression addFilter(IChainFilter filter)
        {
            _add(filter);
            return this;
        }

        /// <summary>
        /// Chain multiple filters together in a boolean 'Or' manner
        /// </summary>
        ChainPredicate IOrExpression.Or
        {
            get
            {
                _add = f => _filters.Add(f);
                return this;
            }
        }

        /// <summary>
        /// Creates a boolean "and" relationship with the previous filter
        /// in this expression.
        /// </summary>
        ChainPredicate IOrExpression.And
        {
            get { 
                _add = and;
                return this;
            }
        }

        private void and(IChainFilter filter)
        {
            var last = _filters.LastOrDefault();
            if (last == null)
            {
                _filters.Add(filter);

                return;
            }

            var and = last as AndChainFilter;
            if (and == null)
            {
                _filters.Remove(last);
                and = new AndChainFilter(last);
                _filters.Add(and);
            }

            and.Add(filter);
        }
    }

    public interface IOrExpression
    {
        ChainPredicate Or { get; }
        ChainPredicate And { get; }
    }
}