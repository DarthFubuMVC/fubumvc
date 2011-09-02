using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Conventions;

namespace FubuMVC.Core
{
    public partial class FubuRegistry
    {
        public void ApplyHandlerConventions()
        {
            ApplyHandlerConventions(GetType());
        }

        public void ApplyHandlerConventions<T>()
            where T : class
        {
            ApplyHandlerConventions(typeof(T));
        }

        public void ApplyHandlerConventions(params Type[] markerTypes)
        {
            markerTypes
                .Each(t => Applies
                               .ToAssembly(t.Assembly));

            includeHandlers(markerTypes);

            Routes
                .UrlPolicy(new HandlersUrlPolicy(markerTypes));
        }

        private void includeHandlers(params Type[] markerTypes)
        {
            markerTypes.Each(markerType => Actions.IncludeTypes(t => t.Namespace.StartsWith(markerType.Namespace)));
            Actions.IncludeMethods(action => action.Method.Name == HandlersUrlPolicy.METHOD);
        }
    }
}