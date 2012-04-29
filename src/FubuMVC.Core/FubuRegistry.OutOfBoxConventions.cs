using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
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
            ApplyHandlerConventions(typeof (T));
        }

        public void ApplyHandlerConventions(params Type[] markerTypes)
        {
            ApplyHandlerConventions(markers => new HandlersUrlPolicy(markers), markerTypes);
        }

        public void ApplyHandlerConventions(Func<Type[], HandlersUrlPolicy> policyBuilder, params Type[] markerTypes)
        {
            markerTypes.Each(t => Applies.ToAssembly(t.Assembly));

            var source = new HandlerActionSource(markerTypes);
            Routes.UrlPolicy(policyBuilder(markerTypes));

            Actions.FindWith(source);
        }

        #region Nested type: HandlerActionSource

        public class HandlerActionSource : ActionSource
        {
            private readonly IEnumerable<Type> _markerTypes;

            public HandlerActionSource(IEnumerable<Type> markerTypes) : base(new ActionMethodFilter())
            {
                markerTypes.Each<Type>(
                    markerType =>
                    {
                        TypeFilters.Includes +=
                            t => t.Namespace.IsNotEmpty() && t.Namespace.StartsWith(markerType.Namespace);
                    });

                MethodFilters.Includes += m => m.Name == HandlersUrlPolicy.METHOD;

                _markerTypes = markerTypes;
            }

            public bool Equals(HandlerActionSource other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;

                if (other._markerTypes.Count() != _markerTypes.Count()) return false;

                for (var i = 0; i < _markerTypes.Count(); i++)
                {
                    if (other._markerTypes.ElementAt(i) != _markerTypes.ElementAt(i)) return false;
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (HandlerActionSource)) return false;
                return Equals((HandlerActionSource) obj);
            }

            public override int GetHashCode()
            {
                return (_markerTypes != null ? _markerTypes.GetHashCode() : 0);
            }
        }

        #endregion
    }
}