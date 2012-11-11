using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Core.Registration
{
    public class Policy : IConfigurationAction
    {
        private readonly IList<IChainModification> _actions = new List<IChainModification>();
        private readonly IList<IChainFilter> _wheres = new List<IChainFilter>();

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

        public AddToEndExpression Add
        {
            get
            {
                return new AddToEndExpression(this);
            }
        }

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

            public IOrExpression Matching<T>() where T : IChainFilter, new()
            {
                return addFilter(new T());
            }

            public IOrExpression LastActionMatches(Func<ActionCall, bool> filter, string description)
            {
                return addFilter(new LastActionMatch(filter, description));
            }

            public IOrExpression LastActionMatches(Expression<Func<ActionCall, bool>> expression)
            {
                return addFilter(new LastActionMatch(expression));
            }

            public IOrExpression AnyActionMatches(Func<ActionCall, bool> filter, string description)
            {
                return addFilter(new AnyActionMatch(filter, description));
            }

            public IOrExpression AnyActionMatches(Expression<Func<ActionCall, bool>> expression)
            {
                return addFilter(new AnyActionMatch(expression));
            } 

            public IOrExpression IsPartialOnly()
            {
                return addFilter(new IsPartial());
            }

            public IOrExpression IsNotPartial()
            {
                return addFilter(new IsNotPartial());
            }

            public IOrExpression ResourceTypeImplements<T>()
            {
                return addFilter(new ResourceTypeImplements<T>());
            }

            public IOrExpression ResourceTypeIs<T>()
            {
                return addFilter(new ResourceTypeIs<T>());
            }

            public IOrExpression InputTypeImplements<T>()
            {
                return addFilter(new InputTypeImplements<T>());
            }

            public IOrExpression InputTypeIs<T>()
            {
                return addFilter(new InputTypeIs<T>());
            }

            public IOrExpression ChainMatches(Expression<Func<BehaviorChain, bool>> expression)
            {
                return addFilter(new LambdaChainFilter(expression));
            }

            public IOrExpression ChainMatches(Func<BehaviorChain, bool> filter, string description)
            {
                return addFilter(new LambdaChainFilter(filter, description));
            }

            private IOrExpression addFilter(IChainFilter filter)
            {
                _register(filter);
                return this;
            }

            public WhereExpression Or
            {
                get { return new WhereExpression(_parent, _parent.registerOrFilter); }
            }
        }

        public void ModifyWith<T>() where T : IChainModification, new()
        {
            _actions.Add(new T());
        }

        public ConnegExpression Conneg
        {
            get
            {
                return new ConnegExpression(this);
            }
        }

        public void ModifyWith(IChainModification chainModification)
        {
            _actions.Add(chainModification);
        }

        public void ModifyBy(Action<BehaviorChain> alteration, string description = "User defined")
        {
            _actions.Add(new LambdaChainModification(alteration)
            {
                Description = description
            });
        }
    }


}