using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public static class EnvironmentTestExtensions
    {
        public static T VerifyRegistration<T>(this IServiceLocator services, IActivationLog log)
        {
            try
            {
                var service = services.GetInstance<T>();
                log.Trace("Using {0} for {1}", service.GetType().FullName, typeof(T).FullName);

                return service;
            }
            catch (Exception ex)
            {
                log.MarkFailure("Could not resolve " + typeof(T).FullName);
                log.MarkFailure(ex);

                return default(T);
            }
        }

        public static IEnumerable<T> VerifyAnyRegistrations<T>(this IServiceLocator services, IActivationLog log)
        {
            try
            {
                var holder = services.GetInstance<Holder<T>>();
                holder.List.Each(x => {
                    log.Trace("Using {0} for {1}", x.GetType().FullName, typeof(T).FullName);
                });


                if (!holder.List.Any())
                {
                    log.MarkFailure("No implementations of {0} are registered", typeof(T).FullName);
                }

                return holder.List;
            }
            catch (Exception ex)
            {
                log.MarkFailure("Could not resolve the list of " + typeof(T).FullName);
                log.MarkFailure(ex);

                return Enumerable.Empty<T>();
            }
        }

        public class Holder<T>
        {
            private readonly IEnumerable<T> _list;

            public Holder(IEnumerable<T> list)
            {
                _list = list;
            }

            public IEnumerable<T> List
            {
                get { return _list; }
            }
        }
    }
}