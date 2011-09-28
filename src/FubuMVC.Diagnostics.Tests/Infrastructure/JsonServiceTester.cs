using System;
using FubuMVC.Diagnostics.Core.Infrastructure;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Infrastructure
{
    [TestFixture]
    public class JsonServiceTester
    {
        [Test]
        public void default_should_be_json_provider()
        {
            JsonService
                .Provider
                .ShouldBeOfType<JsonProvider>();
        }

        [Test]
        public void should_stub_provider()
        {
            JsonService.Stub(new JsonProviderStub());
            JsonService
                .Serialize(new object())
                .ShouldEqual("Test");
        }

        public class JsonProviderStub : IJsonProvider
        {
            public string Serialize(object target)
            {
                return "Test";
            }

            public T Deserialize<T>(string input)
            {
                return (T) Activator.CreateInstance(typeof (T));
            }
        }
    }
}