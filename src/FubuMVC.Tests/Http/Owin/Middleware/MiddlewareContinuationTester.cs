using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Http.Owin.Middleware;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Owin.Middleware
{
    
    public class MiddlewareContinuationTester
    {
        private static IDictionary<string, object> theEnvironment;

        private static Task theInnerTask = Task.Factory.StartNew(() => { });
        private Func<IDictionary<string, object>, Task> theInner = env => {
            env.ShouldBeTheSameAs(theEnvironment);

            return theInnerTask;
        };

        public MiddlewareContinuationTester()
        {
            theEnvironment = new Dictionary<string, object>();
        }

        [Fact]
        public void no_action_and_stop()
        {
            MiddlewareContinuation.StopHere()
                .ToTask(theEnvironment, theInner)
                .ShouldNotBeTheSameAs(theInnerTask);
        }

        [Fact]
        public void an_action_and_stop()
        {
            var wasCalled = false;

            var task = MiddlewareContinuation.StopHere(() => wasCalled = true)
                .ToTask(theEnvironment, theInner);
            task.ShouldNotBeTheSameAs(theInnerTask);

            task.Wait();

            wasCalled.ShouldBeTrue();
        }

        [Fact]
        public void no_action_and_continue()
        {
            MiddlewareContinuation.Continue()
                .ToTask(theEnvironment, theInner)
                .ShouldBeTheSameAs(theInnerTask);
        }

        [Fact]
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