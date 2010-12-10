using System;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class LambdaActivatorTester
    {
        [Test]
        public void to_string_just_repeats_the_ctor_arg()
        {
            var activator = new LambdaActivator("some description", () => { });
            activator.ToString().ShouldEqual("some description");
        }

        [Test]
        public void activate_method_runs_the_action()
        {
            var action = MockRepository.GenerateMock<Action>();

            var activator = new LambdaActivator("something", action);
            activator.Activate(null, null);

            action.AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void activate_generic_lambda()
        {
            string name = "Jeremy";
            var action = MockRepository.GenerateMock<Action<string>>();

            var activator = new LambdaActivator<string>(name, "some description", action);
            activator.Activate(null, null);

            action.AssertWasCalled(x => x.Invoke(name));
        }
    }
}