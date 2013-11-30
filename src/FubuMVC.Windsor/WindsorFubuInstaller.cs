using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Windsor
{
    public class WindsorFubuInstaller : IWindsorInstaller
    {
        readonly IList<Tuple<Type, ObjectDef>> _registrations = new List<Tuple<Type, ObjectDef>>();
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IServiceLocator>().ImplementedBy<WindsorServiceLocator>(),
                Component.For<ISessionState>().ImplementedBy<SimpleSessionState>()
                );

            foreach (var registration in _registrations)
            {
                container.Register(registration.Item1, registration.Item2);
            }
        }

        public void Add(Type type, ObjectDef def)
        {
            _registrations.Add(new Tuple<Type, ObjectDef>(type, def));
        }
    }
}