using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.View.Activation
{
    public interface IPageActivationRules
    {
        IEnumerable<IPageActivationAction> ActivationsFor(Type type);
    }

    public class PageActivationRuleCache : IPageActivationRules
    {
        private readonly Cache<Type, IEnumerable<IPageActivationAction>> _typeRules
            = new Cache<Type, IEnumerable<IPageActivationAction>>();
    
    
        private readonly IList<IPageActivationSource> _sources = new List<IPageActivationSource>();

        public PageActivationRuleCache(IEnumerable<IPageActivationSource> sources)
        {
            _sources.AddRange(sources);
            _typeRules.OnMissing = type => _sources.SelectMany(x => x.ActionsFor(type));
            _sources.Insert(0, new SetPageModelActivationSource());
        }

        public IEnumerable<IPageActivationAction> ActivationsFor(Type type)
        {
            return _typeRules[type];
        }

        public PageActivationExpression IfTheViewTypeMatches(Func<Type, bool> filter)
        {
            return new PageActivationExpression(s => _sources.Add(s), filter);
        }

        public PageActivationExpression IfTheInputModelOfTheViewMatches(Func<Type, bool> filter)
        {
            Func<Type, bool> combined = type =>
            {
                var inputType = type.InputModel();
                return inputType == null ? false : filter(inputType);
            };

            return IfTheViewTypeMatches(combined);
        }
    }
}