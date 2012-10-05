using System;
using FubuMVC.Core.Assets;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class RecordingAssetRegistrationTester
    {
        private IAssetRegistration theUnderlyingRegistration;

        [SetUp]
        public void SetUp()
        {
            theUnderlyingRegistration = MockRepository.GenerateMock<IAssetRegistration>();
        }

        private void IfTheCallsIs(Action<RecordingAssetRegistration> configure)
        {
            var registration = new RecordingAssetRegistration();
            configure(registration);

            registration.Replay(theUnderlyingRegistration);
        }

        [Test]
        public void alias_delegates()
        {
            IfTheCallsIs(x => x.Alias("a.js", "a"));

            theUnderlyingRegistration.AssertWasCalled(x => x.Alias("a.js", "a"));
        }

        [Test]
        public void dependency_delegates()
        {
            IfTheCallsIs(x => x.Dependency("a.js", "b.js"));
            theUnderlyingRegistration.AssertWasCalled(x => x.Dependency("a.js", "b.js"));
        }

        [Test]
        public void extension_delegates()
        {
            IfTheCallsIs(x => x.Extension("a.js", "a-base.js"));
            theUnderlyingRegistration.AssertWasCalled(x => x.Extension("a.js", "a-base.js"));
        }

        [Test]
        public void add_to_set_delegates()
        {
            IfTheCallsIs(x => x.AddToSet("setA", "a.js"));
            theUnderlyingRegistration.AssertWasCalled(x => x.AddToSet("setA", "a.js"));
        }

        [Test]
        public void preceeding_delegates()
        {
            IfTheCallsIs(x => x.Preceeding("before.js", "after.js"));
            theUnderlyingRegistration.AssertWasCalled(x => x.Preceeding("before.js", "after.js"));
        }

        [Test]
        public void combo_delegates()
        {
            IfTheCallsIs(x => x.AddToCombination("a.js", "b.js"));

            theUnderlyingRegistration.AssertWasCalled(x => x.AddToCombination("a.js", "b.js"));
        }

        [Test]
        public void combo_policy_delegates()
        {
            IfTheCallsIs(x => x.ApplyPolicy("CombineEverything"));

            theUnderlyingRegistration.AssertWasCalled(x => x.ApplyPolicy("CombineEverything"));
        }
    }
}