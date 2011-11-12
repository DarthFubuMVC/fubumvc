using System;
using FubuCore.Configuration;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.AppSettings
{
    [TestFixture]
    public class AppSettingsRequestDataTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            data = new AppSettingsRequestData();
        }

        #endregion

        private AppSettingsRequestData data;

        [Test]
        public void exercise_the_callback()
        {
            var callback = MockRepository.GenerateMock<Action<object>>();
            data.Value("FakeSettings.Name", callback);

            callback.AssertWasCalled(x => x.Invoke("Max"));
        }

        [Test]
        public void simply_pull_data()
        {
            // It's hard coded in the FubuMVC.Tests.dll.config file
            data.Value("FakeSettings.Name").ShouldEqual("Max");
        }

        [Test]
        public void call_to_non_existing_key_does_nothing()
        {
            var callback = MockRepository.GenerateMock<Action<object>>();
            data.Value("FakeSettings.Name_NonExisting", callback).ShouldBeFalse();

            callback.AssertWasNotCalled(x => x.Invoke("Max"));
        }

        [Test]
        public void has_any_prefixed_positive()
        {
            data.HasAnyValuePrefixedWith("FakeSettings.").ShouldBeTrue();
        }

        [Test]
        public void has_any_prefixed_negative()
        {
            data.HasAnyValuePrefixedWith("NonExistentKey").ShouldBeFalse();
        }
    }
}