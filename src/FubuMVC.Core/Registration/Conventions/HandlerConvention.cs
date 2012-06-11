using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Registration.Conventions
{
    public class HandlerConvention : IFubuRegistryExtension
    {
        private readonly IList<Type> _markerTypes = new List<Type>();

        public void MarkerType<T>()
        {
            _markerTypes.Fill(typeof(T));
        }

        public Func<Type[], HandlersUrlPolicy> PolicyBuilder = types => new HandlersUrlPolicy(types);

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            IEnumerable<Type> markers = _markerTypes.Any()
                                            ? _markerTypes
                                            : new Type[]{registry.GetType()};

            markers.Each(t => registry.Applies.ToAssembly(t.Assembly));

            var source = new HandlerActionSource(markers);
            registry.Routes.UrlPolicy(PolicyBuilder(markers.ToArray()));

            registry.Actions.FindWith(source);
        }


        public class HandlerActionSource : ActionSource
        {
            private readonly IEnumerable<Type> _markerTypes;

            public HandlerActionSource(IEnumerable<Type> markerTypes)
                : base(new ActionMethodFilter())
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
                if (obj.GetType() != typeof(HandlerActionSource)) return false;
                return Equals((HandlerActionSource)obj);
            }

            public override int GetHashCode()
            {
                return (_markerTypes != null ? _markerTypes.GetHashCode() : 0);
            }
        }

        public void MarkerType(Type markerType)
        {
            _markerTypes.Fill(markerType);
        }
    }
}