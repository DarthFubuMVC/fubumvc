using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Conventions;

namespace FubuMVC.Core
{
    public partial class FubuRegistry
    {
        [Obsolete("Use Import<HandlerConvention>() instead.  Will be removed before 1.0")]
        public void ApplyHandlerConventions()
        {
            Import<HandlerConvention>();
        }

        [Obsolete("Use Import<HandlerConvention>(x => x.MarkerType<T>()) instead.  Will be removed before 1.0")]
        public void ApplyHandlerConventions<T>()
            where T : class
        {
            Import<HandlerConvention>(c => c.MarkerType<T>());
        }

        [Obsolete("Use Import<HandlerConvention>(c => markerTypes.Each(c.MarkerType) instead.  Will be removed before 1.0")]
        public void ApplyHandlerConventions(params Type[] markerTypes)
        {
            Import<HandlerConvention>(c => markerTypes.Each(c.MarkerType));
        }

        [Obsolete("Use Import<HandlerConvention>(Action<HandlerConvention>) instead.  Will be removed before 1.0")]
        public void ApplyHandlerConventions(Func<Type[], HandlersUrlPolicy> policyBuilder, params Type[] markerTypes)
        {
            Import<HandlerConvention>(c =>
            {
                markerTypes.Each(c.MarkerType);
                c.PolicyBuilder = policyBuilder;
            });
        }

    }
}