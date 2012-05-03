using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class BehaviorChainPolicy : IConfigurationAction, DescribesItself
    {
        private readonly IList<IChainFilter> _conditions = new List<IChainFilter>();
        private readonly IList<IChainModification> _modifications = new List<IChainModification>();

        public string Description { get; set; }
        public string Title { get; set; }

        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(chain => _conditions.Any(condition => condition.Matches(chain)))
                .Each(chain => _modifications.Each(x => x.Modify(chain)));
        }

        public void Describe(Description description)
        {
            if (Title != null) description.Title = Title;
            if (Description != null) description.ShortDescription = Description;

            description.AddList("Conditions", _conditions);
            description.AddList("Modifications", _modifications);
        }

        public void Modify(Action<BehaviorChain> modification, string title = null, string description = null)
        {
            var modifier = new LambdaChainModification(modification){
                Title = title,
                Description = description
            };

            _modifications.Add(modifier);
        }

        public void MarkAsPartial()
        {
            Modify(chain =>
            {
                chain.IsPartialOnly = true;
                chain.Route = null;
            }, "Mark as partial");
        }

        public class IfExpression
        {
            private readonly BehaviorChainPolicy _parent;

            public IfExpression(BehaviorChainPolicy parent)
            {
                _parent = parent;
            }

            public InputTypeFilters Input
            {
                get
                {
                    return new InputTypeFilters(_parent, f => _parent._conditions.Add(f));
                }
            }
        }

        public class AndExpression
        {
            private readonly BehaviorChainPolicy _parent;
            private readonly CompoundChainFilter _compoundFilter;

            public AndExpression(BehaviorChainPolicy parent)
            {
                _parent = parent;

                _compoundFilter = _parent._conditions.Last() as CompoundChainFilter;
                if (_compoundFilter == null)
                {
                    _compoundFilter = new CompoundChainFilter();
                    var filter = _parent._conditions.Last();
                    _parent._conditions.Remove(filter);
                    _compoundFilter.Add(filter);
                }
            }

            public InputTypeFilters Input
            {
                get
                {
                    return new InputTypeFilters(_parent, filter => _compoundFilter.Add(filter));
                }
            }
        }

        public class InputTypeFilters
        {
            private readonly BehaviorChainPolicy _parent;
            private readonly Action<IChainFilter> _add;

            public InputTypeFilters(BehaviorChainPolicy parent, Action<IChainFilter> add)
            {
                _parent = parent;
                _add = add;
            }

            private AndExpression condition(Func<Type, bool> filter, string title, string description = null)
            {
                _add(new LambdaChainFilter(chain => filter(chain.InputType())){
                    Description = description,
                    Title = title
                });
                return new AndExpression(_parent);
            }

            public AndExpression CanBeCastTo<T>()
            {
                return condition(type => type.CanBeCastTo<T>(),
                                 "Input type can be cast to " + typeof (T).FullName, "Input type is " + typeof (T).Name);
            }

            public AndExpression HasAny()
            {
                return condition(type => type != null, "Has any input type");
            }

            public AndExpression Closes(Type openType)
            {
                return condition(type => type.Closes(openType), "Input type closes " + openType.Name,
                                 "Input type closes " + openType.PrettyPrint());
            }
        }
    }


    public interface IChainFilter
    {
        bool Matches(BehaviorChain chain);
    }

    public class LambdaChainFilter : IChainFilter, DescribesItself
    {
        private readonly Func<BehaviorChain, bool> _filter;

        public LambdaChainFilter(Func<BehaviorChain, bool> filter)
        {
            _filter = filter;
        }

        public bool Matches(BehaviorChain chain)
        {
            return _filter(chain);
        }

        public string Description { get; set; }
        public string Title { get; set;}

        public void Describe(Description description)
        {
            if (Title != null) description.Title = Title;
            if (Description != null) description.ShortDescription = Description;
        }
    }

    public class CompoundChainFilter : IChainFilter
    {
        private readonly IList<IChainFilter> _filters = new List<IChainFilter>();

        public void Add(IChainFilter filter)
        {
            _filters.Add(filter);
        }

        public IEnumerable<IChainFilter> Filters
        {
            get { return _filters; }
        }

        public bool Matches(BehaviorChain chain)
        {
            return _filters.All(x => x.Matches(chain));
        }
    }


    public interface IChainModification
    {
        void Modify(BehaviorChain chain);
    }

    public class LambdaChainModification : IChainModification, DescribesItself
    {
        private readonly Action<BehaviorChain> _modification;

        public LambdaChainModification(Action<BehaviorChain> modification)
        {
            _modification = modification;
        }

        public void Modify(BehaviorChain chain)
        {
            _modification(chain);
        }

        public string Description { get; set; }
        public string Title { get; set; }

        public void Describe(Description description)
        {
            if (Title != null) description.Title = Title;
            if (Description != null) description.ShortDescription = Description;
        }
    }
}