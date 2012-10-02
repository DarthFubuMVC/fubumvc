using System;
using FubuCore;

namespace FubuMVC.Core.View.Activation
{
    public class PageActivationExpression
    {
        private readonly FubuRegistry _registry;
        private readonly Func<IViewToken, bool> _filter;


        public PageActivationExpression(FubuRegistry registry, Func<IViewToken, bool> filter)
        {
            _registry = registry;
            _filter = filter;
        }

        ///// <summary>
        ///// For the given set of views, specify a <see cref="IPageActivationAction"/> instance
        ///// </summary>
        //public void ActivateBy(IPageActivationAction action)
        //{
        //    _registration(new LambdaPageActivationSource(_filter, type => action));
        //}

        ///// <summary>
        ///// For the given set of views, access the instantiated <see cref="IFubuPage"/>
        ///// </summary>
        ///// <param name="action"></param>
        //public void ActivateBy(Action<IFubuPage> action)
        //{
        //    ActivateBy(new LambdaPageActivationAction(action));
        //}

        ///// <summary>
        ///// For the given set of views, specify any service T from the services known at runtime.
        ///// It subsequently gets the chance to interact with the <see cref="IFubuPage"/> instance.
        ///// </summary>
        //public void ActivateBy<T>(Action<T, IFubuPage> action)
        //{
        //    throw new NotImplementedException();
        //    //ActivateBy(new LambdaPageActivationAction((s, p) => action(s.GetInstance<T>(), p)));                
        //}

        /// <summary>
        /// Sets the profile name for Html conventions
        /// </summary>
        /// <param name="profileName"></param>
        public void SetTagProfileTo(string profileName)
        {
            throw new NotImplementedException();
        }
    }
}