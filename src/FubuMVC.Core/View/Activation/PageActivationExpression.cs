using System;
using FubuCore;

namespace FubuMVC.Core.View.Activation
{
    public class PageActivationExpression
    {
        private readonly Action<IPageActivationSource> _registration;
        private readonly Func<Type, bool> _filter;

        public PageActivationExpression(Action<IPageActivationSource> registration, Func<Type, bool> filter)
        {
            _registration = registration;
            _filter = filter;
        }

        public void ActivateBy(IPageActivationAction action)
        {
            _registration(new LambdaPageActivationSource(_filter, type => action));
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

            _registration(source);
        }
    }
}