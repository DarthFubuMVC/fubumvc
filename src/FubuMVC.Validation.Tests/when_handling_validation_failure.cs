using System;
using System.Collections.Generic;
using FubuMVC.Core.Runtime;
using FubuValidation;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap.AutoMocking;

namespace FubuMVC.Validation.Tests
{
    [TestFixture]
    public class when_handling_validation_failure
    {
        private ValidationFailureHandler _handler;
        private List<IValidationFailurePolicy> _policies;

        [SetUp]
        public void SetUp()
        {
            var services = new RhinoAutoMocker<SampleInputModel>(MockMode.AAA);
            var request = services.Get<IFubuRequest>();

            _policies = new List<IValidationFailurePolicy>();
            _handler = new ValidationFailureHandler(_policies, request);
        }

        [Test]
        public void should_throw_validation_exception_if_no_policies_are_found()
        {
            Exception<FubuMVCValidationException>
                .ShouldBeThrownBy(() => _handler.Handle(typeof(SampleInputModel)));
        }

        [Test]
        public void should_invoke_first_policy_that_is_matched()
        {
            _policies.Add(new SampleValidationFailurePolicy(() => { }));
            _policies.Add(new SampleValidationFailurePolicy(() => Assert.Fail("Invalid policy invoked")));

            _handler.Handle(typeof(SampleInputModel));
        }

        public class SampleValidationFailurePolicy : IValidationFailurePolicy
        {
            private readonly Action _continuation;

            public SampleValidationFailurePolicy(Action continuation)
            {
                _continuation = continuation;
            }

            public bool Matches(Type modelType)
            {
                return true;
            }

            public void Handle(Type modelType, Notification notification)
            {
                _continuation();
            }
        }
    }
}