using System;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

namespace FubuMVC.Tests.Registration.ObjectGraph
{
    [TestFixture]
    public class ConfiguredDependencyTester
    {
        [Test]
        public void assert_valid_with_value_happy_path()
        {
            var dependency = new ConfiguredDependency(typeof (IService), typeof(ConcreteService));

            dependency.AssertValid();
        }

        [Test]
        public void assert_valid_with_value_does_not_cast()
        {
            Exception<ObjectDefException>.ShouldBeThrownBy(() =>
            {
                var dependency = new ConfiguredDependency(typeof (IService), new NotAService());

                dependency.AssertValid();
            }).Message.ShouldEqual("Object of type {0} can not be cast to {1}".ToFormat(typeof(NotAService).FullName, typeof(IService).FullName));
        }

        [Test]
        public void assert_valid__with_type_happy_path()
        {
            var dependency = ConfiguredDependency.For<IService, ConcreteService>();
            dependency.AssertValid();
        }

        [Test]
        public void assert_valid_with_invalid_cast()
        {
            Exception<ObjectDefException>.ShouldBeThrownBy(() =>
            {
                var dependency = new ConfiguredDependency(typeof (IService),typeof (NotAService));
                dependency.AssertValid();
            }).Message.ShouldEqual("{0} cannot be cast to {1}".ToFormat(typeof(NotAService).FullName, typeof(IService).FullName));
        }

        [Test]
        public void assert_valid_with_no_value_or_type()
        {
            Type concreteTypeThatIsNull = null;

            Exception<ObjectDefException>.ShouldBeThrownBy(() =>
            {
                new ConfiguredDependency(typeof(IService), concreteTypeThatIsNull).AssertValid();
            });
        }

        [Test]
        public void assert_valid_with_abstract_type()
        {
            Exception<ObjectDefException>.ShouldBeThrownBy(() =>
            {
                ConfiguredDependency.For<IService, AbstractService>().AssertValid();
            }).Message.ShouldEqual("{0} is not a concrete type.".ToFormat(typeof(AbstractService).FullName));
        }
    }

    public interface IService{}
    public abstract class AbstractService : IService{}
    public class ConcreteService : IService{}
    public class NotAService{}

    public class ArbitraryConcreteClass
    {
        // I should be able to resolve or "get" this from 
        // the IoC container, with the IService dependency resolved,
        // without having to explicitly register ArbitraryConcreteClass
        public ArbitraryConcreteClass(IService dependency)
        {
        }
    }
}