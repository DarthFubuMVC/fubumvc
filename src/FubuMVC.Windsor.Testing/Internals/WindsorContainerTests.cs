using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace FubuMVC.Windsor.Testing.Internals
{
    [TestFixture]
    public class WindsorContainerTests
    {
        [Test]
        public void windsor_resolves_first_registered_service()
        {
            var container = new WindsorContainer();
            
            container.Register(
                Component.For<ITestService>().ImplementedBy<TestService1>(),
                Component.For<ITestService>().ImplementedBy<TestService2>()
                );

            var service = container.Resolve<ITestService>();
            Assert.AreEqual(typeof(TestService1), service.GetType());
        }

        [Test]
        public void windsor_resolves_default_service()
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For<ITestService>().ImplementedBy<TestService1>(),
                Component.For<ITestService>().ImplementedBy<TestService2>().IsDefault()
                );

            var service = container.Resolve<ITestService>();
            Assert.AreEqual(typeof(TestService2), service.GetType());
        }

        [Test]
        public void windsor_not_resolves_fallback_service_registered_first()
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For<ITestService>().ImplementedBy<TestService1>().IsFallback(),
                Component.For<ITestService>().ImplementedBy<TestService2>()
                );

            var service = container.Resolve<ITestService>();
            Assert.AreEqual(typeof(TestService2), service.GetType());
        }

        [Test]
        public void windsor_not_resolves_fallback_service_registered_second()
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For<ITestService>().ImplementedBy<TestService1>(),
                Component.For<ITestService>().ImplementedBy<TestService2>().IsFallback()
                );

            var service = container.Resolve<ITestService>();
            Assert.AreEqual(typeof(TestService1), service.GetType());
        }

        [Test]
        public void windsor_not_resolves_named_fallback_service_registered_second()
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For<ITestService>().ImplementedBy<TestService1>(),
                Component.For<ITestService>().ImplementedBy<TestService2>().Named("s1").IsFallback()
                );

            var service = container.Resolve<ITestService>("s1");
            Assert.AreEqual(typeof(TestService2), service.GetType());
        }
    }

    internal interface ITestService
    {
    }

    internal class TestService1 : ITestService
    {
    }

    internal class TestService2 : ITestService
    {
    }
}
