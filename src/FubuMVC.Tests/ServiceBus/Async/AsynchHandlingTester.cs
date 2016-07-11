using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus.Async;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Async
{
    [TestFixture]
    public class AsynchHandlingTester
    {
        [Test]
        public void finish_successfully()
        {
            var handling = new AsyncHandling(ObjectMother.InvocationContext());
            var list = new ConcurrentBag<string>();

            var task1 = Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                list.Add("A");
            });

            var task2 = Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                list.Add("B");
            });

            var task3 = Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                list.Add("C");
            });

            handling.Push(task1);
            handling.Push(task2);
            handling.Push(task3);

            handling.WaitForAll();

            list.OrderBy(x => x).ShouldHaveTheSameElementsAs("A", "B", "C");
        }


        [Test]
        public void records_outgoing_on_the_invocation_context()
        {
            var context = ObjectMother.InvocationContext();
            var handling = new AsyncHandling(context);

            var expectedMessage = new Message1();

            var task1 = Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                return expectedMessage;
            });

            handling.Push(task1);

            handling.WaitForAll();

            context.OutgoingMessages().Single()
                .ShouldBeTheSameAs(expectedMessage);
        }

        [Test]
        public void records_outgoing_on_the_invocation_context_for_object_array()
        {
            var context = ObjectMother.InvocationContext();
            var handling = new AsyncHandling(context);

            var expectedMessages = new object[] {new Message1(), new Message2(), new Message3()};

            var task1 = Task.Factory.StartNew(() => { return expectedMessages; });

            handling.Push(task1);

            handling.WaitForAll();
            context.OutgoingMessages().ShouldHaveTheSameElementsAs(expectedMessages);
        }
    }
}