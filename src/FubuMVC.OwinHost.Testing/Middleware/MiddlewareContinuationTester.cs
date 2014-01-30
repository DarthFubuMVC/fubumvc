using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.OwinHost.Middleware;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.OwinHost.Testing.Middleware
{
    [TestFixture]
    public class MiddlewareContinuationTester
    {
        private static IDictionary<string, object> theEnvironment;

        private static bool innerWasCalled;
        private static Task theInnerTask = Task.Factory.StartNew(() => { });
        private Func<IDictionary<string, object>, Task> theInner = env => {
            env.ShouldBeTheSameAs(theEnvironment);

            return theInnerTask;
        };

        [SetUp]
        public void SetUp()
        {
            innerWasCalled = false;
            theEnvironment = new Dictionary<string, object>();
        }

        [Test]
        public void no_action_and_stop()
        {
            MiddlewareContinuation.StopHere()
                .ToTask(theEnvironment, theInner)
                .ShouldNotBeTheSameAs(theInnerTask);
        }

        [Test]
        public void an_action_and_stop()
        {
            var wasCalled = false;

            var task = MiddlewareContinuation.StopHere(() => wasCalled = true)
                .ToTask(theEnvironment, theInner);
            task.ShouldNotBeTheSameAs(theInnerTask);

            task.Wait();

            wasCalled.ShouldBeTrue();
        }

        [Test]
        public void no_action_and_continue()
        {
            MiddlewareContinuation.Continue()
                .ToTask(theEnvironment, theInner)
                .ShouldBeTheSameAs(theInnerTask);
        }

        [Test]
        public void an_action_and_continue()
        {
            var wasCalled = false;

            var task = MiddlewareContinuation.Continue(() => wasCalled = true)
                .ToTask(theEnvironment, theInner)
                .ShouldBeTheSameAs(theInnerTask);

            wasCalled.ShouldBeTrue();
        }
    }
}