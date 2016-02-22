using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Localization
{
    [TestFixture]
    public class LocalizationManagerTester
    {
        private MockRepository _mocks;
        private ILocalizationDataProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
            _provider = _mocks.StrictMock<ILocalizationDataProvider>();

            LocalizationManager.Stub(_provider);
        }

        [TearDown]
        public void TearDown()
        {
            LocalizationManager.Stub();
        }

        [Test]
        public void GetText_for_property()
        {
            PropertyInfo property = ReflectionHelper.GetProperty<DummyEntity>(c => c.SimpleProperty);

            using (_mocks.Record())
            {
                Expect.Call(_provider.GetHeader(property)).Return("TheText");
            }

            using (_mocks.Playback())
            {
                LocalizationManager.GetText(property).ShouldBe("TheText");
            }
        }

        [Test]
        public void GetText_for_property_by_expression()
        {
            PropertyInfo property = ReflectionHelper.GetProperty<DummyEntity>(c => c.SimpleProperty);

            using (_mocks.Record())
            {
                Expect.Call(_provider.GetHeader(property)).Return("TheText");
            }

            using (_mocks.Playback())
            {
                LocalizationManager.GetText<DummyEntity>(c => c.SimpleProperty).ShouldBe("TheText");
            }
        }

        [Test]
        public void GetHeader_for_property()
        {
            PropertyInfo property = ReflectionHelper.GetProperty<DummyEntity>(c => c.SimpleProperty);
            var theHeader = "TheText";

            using (_mocks.Record())
            {
                Expect.Call(_provider.GetHeader(property)).Return(theHeader);
            }

            using (_mocks.Playback())
            {
                LocalizationManager.GetHeader(property).ShouldBe(theHeader);
            }
        }

        [Test]
        public void GetHeader_for_property_by_expression()
        {
            PropertyInfo property = ReflectionHelper.GetProperty<DummyEntity>(c => c.SimpleProperty);
            var theHeader = "TheText";

            using (_mocks.Record())
            {
                Expect.Call(_provider.GetHeader(property)).Return(theHeader);
            }

            using (_mocks.Playback())
            {
                LocalizationManager.GetHeader<DummyEntity>(c => c.SimpleProperty).ShouldBe(theHeader);
            }
        }

        public class DummyEntity
        {
            public string SimpleProperty { get; set; }
        }
    }
}