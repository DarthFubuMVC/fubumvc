using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.View.Activation
{
    public class PageActivationRules
    {
        private readonly Cache<Type, IEnumerable<IPageActivationAction>> _typeRules
            = new Cache<Type, IEnumerable<IPageActivationAction>>();
    
    
        private readonly IList<IPageActivationSource> _sources = new List<IPageActivationSource>();

        public PageActivationRules()
        {
            _typeRules.OnMissing = type => _sources.SelectMany(x => x.ActionsFor(type));
            _sources.Add(new SetPageModelActivationSource());
        }

        public IEnumerable<IPageActivationAction> ActivationsFor(Type type)
        {
            return _typeRules[type];
        }

        public PageActivationExpression IfTheViewTypeMatches(Func<Type, bool> filter)
        {
            return new PageActivationExpression(this, filter);
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

        public class PageActivationExpression
        {
            private readonly PageActivationRules _parent;
            private readonly Func<Type, bool> _filter;

            public PageActivationExpression(PageActivationRules parent, Func<Type, bool> filter)
            {
                _parent = parent;
                _filter = filter;
            }

            public void ActivateBy(IPageActivationAction action)
            {
                _parent._sources.Add(new LambdaPageActivationSource(_filter, type => action));
            }

            public void ActivateBy(Action<IFubuPage> action)
            {
                ActivateBy(new LambdaPageActivationAction(action));
            }

            public void ActivateBy<T>(Action<T, IFubuPage> action)
            {
                ActivateBy(new LambdaPageActivationAction((s, p) => action(s.GetInstance<T>(), p)));                
            }

            public void SetTagProfileTo(string profileName)
            {
                var source = new LambdaPageActivationSource(_filter, type =>
                {
                    var inputType = type.InputModel();
                    return
                        typeof (SetTagProfilePageActivationAction<>)
                            .CloseAndBuildAs<IPageActivationAction>(profileName, inputType);
                });

                _parent._sources.Add(source);
            }
        }
    }
}