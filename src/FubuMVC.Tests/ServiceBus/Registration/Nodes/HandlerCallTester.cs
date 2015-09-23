using System;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using NUnit.Framework;
using Shouldly;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Registration.Nodes
{
    [TestFixture]
    public class HandlerCallTester
    {
        [Test]
        public void handler_call_should_not_match_property_setters()
        {
            var handlerType = typeof (ITargetHandler);
            var property = handlerType.GetProperty("Message");
            var method = property.GetSetMethod();
            HandlerCall.IsCandidate(method).ShouldBeFalse();
        }

        [Test]
        public void choose_handler_type_for_one_in_one_out()
        {
            var handler = HandlerCall.For<ITargetHandler>(x => x.OneInOneOut(null));

            var objectDef = handler.As<IContainerModel>().ToInstance();

            objectDef.ReturnedType.ShouldBe(typeof (CascadingHandlerInvoker<ITargetHandler, Input, Output>));
        }

        [Test]
        public void choose_handler_type_for_one_in_zero_out()
        {
            var handler = HandlerCall.For<ITargetHandler>(x => x.OneInZeroOut(null));

            var objectDef = handler.As<IContainerModel>().ToInstance();

            objectDef.ReturnedType.ShouldBe(typeof (SimpleHandlerInvoker<ITargetHandler, Input>));
        }

        [Test]
        public void choose_handler_type_for_call_that_returns_Task()
        {
            var handler = HandlerCall.For<TaskHandler>(x => x.Go(null));

            var objectDef = handler.As<IContainerModel>().ToInstance();

            objectDef.ReturnedType.ShouldBe(typeof (AsyncHandlerInvoker<TaskHandler, Message>));
        }

        [Test]
        public void choose_handler_type_for_call_that_returns_Task_of_T()
        {
            var handler = HandlerCall.For<TaskHandler>(x => x.Other(null));

            var objectDef = handler.As<IContainerModel>().ToInstance();

            objectDef.ReturnedType.ShouldBe(typeof (CascadingAsyncHandlerInvoker<TaskHandler, Message, Message1>));
        }


        [Test]
        public void throws_chunks_if_you_try_to_use_a_method_with_no_inputs()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                () => { HandlerCall.For<ITargetHandler>(x => x.ZeroInZeroOut()); });
        }

        [Test]
        public void is_candidate()
        {
            HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.ZeroInZeroOut())).ShouldBeFalse();
            HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.OneInOneOut(null))).ShouldBeTrue();
            HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.OneInZeroOut(null)))
                .ShouldBeTrue();
            HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.ManyIn(null, null)))
                .ShouldBeFalse();
            HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.ReturnsValueType(null)))
                .ShouldBeFalse();
        }

        [Test]
        public void is_candidate_allows_interface_return_types()
        {
            HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.ReturnsInterface(null)))
                .ShouldBeTrue();
        }

        [Test]
        public void could_handle()
        {
            var handler1 = HandlerCall.For<SomeHandler>(x => x.Interface(null));
            var handler2 = HandlerCall.For<SomeHandler>(x => x.BaseClass(null));

            handler1.CouldHandleOtherMessageType(typeof (Input1)).ShouldBeTrue();
            handler2.CouldHandleOtherMessageType(typeof (Input1)).ShouldBeTrue();

            handler1.CouldHandleOtherMessageType(typeof (Input2)).ShouldBeFalse();
            handler1.CouldHandleOtherMessageType(typeof (Input2)).ShouldBeFalse();
        }

        [Test]
        public void could_handle_is_false_for_its_own_input_type()
        {
            var handler = HandlerCall.For<ITargetHandler>(x => x.OneInOneOut(null));
            handler.CouldHandleOtherMessageType(typeof (Input)).ShouldBeFalse();
        }

        [Test]
        public void handler_equals()
        {
            var handler1 = HandlerCall.For<SomeHandler>(x => x.Interface(null));
            var handler2 = HandlerCall.For<SomeHandler>(x => x.Interface(null));
            var handler3 = HandlerCall.For<SomeHandler>(x => x.Interface(null));

            handler1.ShouldBe(handler2);
            handler1.ShouldBe(handler3);
            handler3.ShouldBe(handler2);
            handler2.ShouldBe(handler1);
        }

        [Test]
        public void handler_is_async_negative()
        {
            HandlerCall.For<SomeHandler>(x => x.Interface(null)).IsAsync.ShouldBeFalse();
        }

        [Test]
        public void handler_is_async_positive()
        {
            HandlerCall.For<TaskHandler>(x => x.Go(null)).IsAsync.ShouldBeTrue();
            HandlerCall.For<TaskHandler>(x => x.Other(null)).IsAsync.ShouldBeTrue();
        }

        public class TaskHandler
        {
            public Task Go(Message message)
            {
                return null;
            }

            public Task<Message1> Other(Message message)
            {
                return null;
            }
        }
    }
}