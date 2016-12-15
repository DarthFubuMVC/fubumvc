﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Http.Owin.Middleware;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin.Middleware
{
    [TestFixture]
    public class MiddlewareContinuationTester
    {
        private static IDictionary<string, object> theEnvironment;

        private static Task theInnerTask = Task.Factory.StartNew(() => { });
        private Func<IDictionary<string, object>, Task> theInner = env => {
            env.ShouldBeTheSameAs(theEnvironment);

            return theInnerTask;
        };

        [SetUp]
        public void SetUp()
        {
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

            var task = MiddlewareContinuation.StopHere(() => {
                wasCalled = true;
                return Task.CompletedTask;
            })
                .ToTask(theEnvironment, theInner);

            task.Wait();

            wasCalled.ShouldBeTrue();
        }


        [Test]
        public void an_action_and_continue()
        {
            var wasCalled = false;

            MiddlewareContinuation.Continue(() => {
                wasCalled = true;
                return Task.CompletedTask;
            })
                .ToTask(theEnvironment, theInner).GetAwaiter().GetResult();

            wasCalled.ShouldBeTrue();
        }
    }
}