using System;
using System.Collections.Generic;
using System.Web;

using Autofac;

using FubuCore;

using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;


namespace FubuMVC.Autofac
{
    public class AutofacFubuModule : Module
    {
        private readonly List<Action<ContainerBuilder>> _registrations = new List<Action<ContainerBuilder>>();


        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => BuildRequestWrapper()).As<HttpRequestWrapper>();
            builder.Register(c => new HttpContextWrapper(BuildContextWrapper())).As<HttpContextBase>();

            builder.RegisterType<AutofacServiceLocator>().As<IServiceLocator>();
            builder.RegisterType<SimpleSessionState>().As<ISessionState>();

            foreach (Action<ContainerBuilder> registration in _registrations)
            {
                registration(builder);
            }
        }


        public void AddRegistration(Type abstraction, ObjectDef definition, bool isSingleton)
        {
            _registrations.Add(
                builder =>
                {
                    var registration = new ObjectDefRegistration(builder, definition, isSingleton, false);
                    registration.Register(abstraction);
                });
        }


        public static HttpContext BuildContextWrapper()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current;
                }
            }
            catch (HttpException)
            {
                // This is only here for web startup when HttpContext.Current is not available.
            }

            return null;
        }

        public static HttpRequestWrapper BuildRequestWrapper()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    return new HttpRequestWrapper(HttpContext.Current.Request);
                }
            }
            catch (HttpException)
            {
                // This is only here for web startup when HttpContext.Current is not available.
            }

            return null;
        }
    }
}