using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Tests.Registration.Expressions;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    public interface IService<T>
    {
    }

    [TestFixture]
    public class TypeExtensionsTester
    {
        public class Service1 : IService<string>
        {
        }

        public class Service2
        {
        }

        public class Service2<T> : IService<T>
        {
        }

        public interface ServiceInterface : IService<string>
        {
        }

        [Test]
        public void closes_applies_to_concrete_types()
        {
            typeof(Service2<string>).Closes(typeof(Service2<>)).ShouldBeTrue();
        }

        [Test]
        public void find_interface_that_closes_open_interface()
        {
            typeof (Service1).FindInterfaceThatCloses(typeof (IService<>))
                .ShouldEqual(typeof (IService<string>));

            typeof (Service2).FindInterfaceThatCloses(typeof (IService<>))
                .ShouldBeNull();
        }

        [Test]
        public void implements_interface_template()
        {
            typeof (Service1).ImplementsInterfaceTemplate(typeof (IService<>))
                .ShouldBeTrue();

            typeof (Service2).ImplementsInterfaceTemplate(typeof (IService<>))
                .ShouldBeFalse();

            typeof (ServiceInterface).ImplementsInterfaceTemplate(typeof (IService<>))
                .ShouldBeFalse();
        }

        [Test]
        public void IsPrimitive()
        {
            Assert.IsTrue(typeof (int).IsPrimitive());
            Assert.IsTrue(typeof (bool).IsPrimitive());
            Assert.IsTrue(typeof (double).IsPrimitive());
            Assert.IsFalse(typeof (string).IsPrimitive());
            Assert.IsFalse(typeof (when_explicitly_registering_a_route.OutputModel).IsPrimitive());
            Assert.IsFalse(typeof (IRouteVisitor).IsPrimitive());
        }

        [Test]
        public void IsSimple()
        {
            Assert.IsTrue(typeof (int).IsSimple());
            Assert.IsTrue(typeof (bool).IsSimple());
            Assert.IsTrue(typeof (double).IsSimple());
            Assert.IsTrue(typeof (string).IsSimple());
            Assert.IsTrue(typeof (DoNext).IsSimple());
            Assert.IsFalse(typeof (IRouteVisitor).IsSimple());
        }

        [Test]
        public void IsString()
        {
            Assert.IsTrue(typeof (string).IsString());
            Assert.IsFalse(typeof (int).IsString());
        }
    }
}