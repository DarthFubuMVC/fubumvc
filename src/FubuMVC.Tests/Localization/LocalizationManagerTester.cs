using System;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Localization
{
    
    public class LocalizationManagerTester : IDisposable
    {
        private MockRepository _mocks;
        private ILocalizationDataProvider _provider;

        public LocalizationManagerTester()
        {
            _mocks = new MockRepository();
            _provider = _mocks.StrictMock<ILocalizationDataProvider>();

            LocalizationManager.Stub(_provider);
        }

        public void Dispose()
        {
            LocalizationManager.Stub();
        }


        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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