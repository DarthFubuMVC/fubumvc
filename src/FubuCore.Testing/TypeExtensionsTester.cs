using System;
using System.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore.Testing;
using Rhino.Mocks;

namespace FubuCore.Testing
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

        public class Service3 : Service3<string>
        {   
        }

        public class Service3<T>
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
        public void closes_applies_to_implementing_an_open_interface()
        {
            typeof (ConcreteListener).Closes(typeof (IListener<>)).ShouldBeTrue();
        }

        public void closes_applies_to_concrete_non_generic_types()
        {
            typeof(Service2).Closes(typeof(Service2<>)).ShouldBeFalse();
        }

        [Test]
        public void closes_applies_to_concrete_non_generic_types_with_generic_parent()
        {
            typeof(Service3).Closes(typeof(Service3<>)).ShouldBeTrue();
            typeof(Service3).Closes(typeof(Service2<>)).ShouldBeFalse();
        }

        [Test]
        public void find_interface_that_closes_open_interface()
        {
            typeof (Service1).FindInterfaceThatCloses(typeof (IService<>))
                .ShouldEqual(typeof (IService<string>));

            typeof (Service2).FindInterfaceThatCloses(typeof (IService<>))
                .ShouldBeNull();

            typeof(IService<>).FindInterfaceThatCloses(Arg<Type>.Is.Anything).ShouldBeNull();
        }

        [Test]
        public void find_parameter_type_to()
        {
            typeof (Service1).FindParameterTypeTo(typeof (IService<>)).ShouldEqual(typeof (string));
            typeof (Service2).FindParameterTypeTo(typeof (IService<>)).ShouldBeNull();


        }

        [Test]
        public void find_interface_that_closes_open_interface_from_another_interface()
        {
            typeof (TestHandler).FindInterfaceThatCloses(typeof (IMessageHandler<>)).ShouldEqual(
                typeof (IMessageHandler<string>));
        }

        public interface IMessageHandler<T>{}
        public interface TestHandler : IMessageHandler<string>{}

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
            Assert.IsFalse(typeof (OutputModel).IsPrimitive());
            Assert.IsFalse(typeof (IRouteVisitor).IsPrimitive());
        }

        public class OutputModel
        {
            
        }

        public interface IRouteVisitor{}

        public enum DoNext{}

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

        [Test]
        public void is_nullable_of_T()
        {
            typeof(string).IsNullableOfT().ShouldBeFalse();
            typeof(Nullable<int>).IsNullableOfT().ShouldBeTrue();
        }

        [Test]
        public void is_nullable_of_a_given_type()
        {
            typeof(string).IsNullableOf(typeof(int)).ShouldBeFalse();
            typeof(Nullable<DateTime>).IsNullableOf(typeof(int)).ShouldBeFalse();
            typeof(Nullable<int>).IsNullableOf(typeof(int)).ShouldBeTrue();
        }

        [Test]
        public void is_type_or_nullable_of_T()
        {
            typeof(bool).IsTypeOrNullableOf<bool>().ShouldBeTrue();
            typeof(Nullable<bool>).IsTypeOrNullableOf<bool>().ShouldBeTrue();
            typeof(Nullable<DateTime>).IsTypeOrNullableOf<bool>().ShouldBeFalse();
            typeof(string).IsTypeOrNullableOf<bool>().ShouldBeFalse();
        
        }

        [Test]
        public void can_be_cast_to()
        {
            typeof(Message1).CanBeCastTo<IMessage>().ShouldBeTrue();
            typeof(Message2).CanBeCastTo<IMessage>().ShouldBeTrue();
            typeof(Message2).CanBeCastTo<Message1>().ShouldBeTrue();

            typeof(Message1).CanBeCastTo<Message1>().ShouldBeTrue();
            typeof(Message1).CanBeCastTo<Message2>().ShouldBeFalse();
            ((Type)null).CanBeCastTo<Message1>().ShouldBeFalse();
        }

        [Test]
        public void is_in_namespace()
        {
            this.GetType().IsInNamespace("wrong").ShouldBeFalse();
            this.GetType().IsInNamespace(this.GetType().Namespace + ".something").ShouldBeFalse();
            this.GetType().IsInNamespace(this.GetType().Namespace).ShouldBeTrue();
            this.GetType().IsInNamespace(this.GetType().Assembly.GetName().Name).ShouldBeTrue();
            
        }

        [Test]
        public void is_open_generic()
        {
            typeof(int).IsOpenGeneric().ShouldBeFalse();
            typeof(Nullable<>).IsOpenGeneric().ShouldBeTrue();
            typeof(Nullable<int>).IsOpenGeneric().ShouldBeFalse();
        }

        [Test]
        public void is_concrete_type_of_T()
        {
            typeof(IMessage).IsConcreteTypeOf<IMessage>().ShouldBeFalse();
            typeof(AbstractMessage).IsConcreteTypeOf<IMessage>().ShouldBeFalse();
            typeof(Message1).IsConcreteTypeOf<IMessage>().ShouldBeTrue();
            typeof(Message2).IsConcreteTypeOf<IMessage>().ShouldBeTrue();
            typeof(Message3).IsConcreteTypeOf<IMessage>().ShouldBeTrue();
            this.GetType().IsConcreteTypeOf<IMessage>().ShouldBeFalse();
        }

        [Test]
        public void is_nullable()
        {
            typeof(string).IsNullable().ShouldBeFalse();
            typeof(Nullable<int>).IsNullable().ShouldBeTrue();
        }

        [Test]
        public void get_inner_type_from_nullable()
        {
            typeof (Nullable<int>).GetInnerTypeFromNullable().ShouldEqual(typeof (int));
        }

        [Test]
        public void get_name_from_generic()
        {
            typeof(Service2<int>).GetName().ShouldEqual("Service2`1<Int32>");
        }

        [Test]
        public void get_full_name_from_generic()
        {
            typeof(Service2<int>).GetFullName().ShouldEqual("Service2`1<Int32>");
        }

        [Test]
        public void get_full_name()
        {
            typeof (string).GetFullName().ShouldEqual("System.String");
        }

        [Test]
        public void is_concrete()
        {
            typeof(IMessage).IsConcrete().ShouldBeFalse();
            typeof(AbstractMessage).IsConcrete().ShouldBeFalse();
            typeof(Message2).IsConcrete().ShouldBeTrue();
        }

        [Test]
        public void is_not_concrete()
        {
            typeof(Message2).IsNotConcrete().ShouldBeFalse();
        }

        [Test]
        public void close_and_build_as()
        {
            var message = typeof (OpenClass<>).CloseAndBuildAs<IMessage>(typeof (string));
            message.ShouldBeOfType<OpenClass<string>>();
            message.ShouldNotBeNull();
        }

        [Test]
        public void is_concrete_with_default_ctor()
        {
            typeof(Message1).IsConcreteWithDefaultCtor().ShouldBeTrue();
            typeof(IMessage).IsConcreteWithDefaultCtor().ShouldBeFalse();

            typeof(ClassWithGreedyCtor).IsConcreteWithDefaultCtor().ShouldBeFalse();
        }

        public interface IMessage{}
        public abstract class AbstractMessage : IMessage{}
        public class Message3 : AbstractMessage{}
        public class Message1 : IMessage{}
        public class Message2 : Message1{}

        public interface IListener<T>{}
        public class ConcreteListener : IListener<string>{}
    
        public class ClassWithGreedyCtor
        {
            public ClassWithGreedyCtor(string name)
            {
                Debug.WriteLine(name);
            }
        }
        

        public class OpenClass<T> : IMessage{}
    }

    
}