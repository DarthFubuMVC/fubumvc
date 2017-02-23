using System;
using FubuMVC.Core.Behaviors;
using Xunit;

namespace FubuMVC.Tests.Behaviors
{
    public abstract class BehaviorTestContext<BEHAVIOR> : IDisposable
        where BEHAVIOR : IActionBehavior
    {
        protected BEHAVIOR _behavior;
        protected TestController _controller;
        protected object _input;
        protected TestOutputModel _outputModel;

        protected BehaviorTestContext()
        {
            _behavior = CreateBehavior();
            _controller = new TestController();
            _input = new TestInputModel();
            _outputModel = new TestOutputModel();

            beforeEach();
        }

        protected virtual BEHAVIOR CreateBehavior()
        {
            return Activator.CreateInstance<BEHAVIOR>();
        }

        public void Dispose()
        {
            afterEach();
        }


        protected virtual void beforeEach()
        {
        }

        protected virtual void afterEach()
        {
        }
    }
}