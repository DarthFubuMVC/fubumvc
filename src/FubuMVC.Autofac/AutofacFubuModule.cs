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
    }
}