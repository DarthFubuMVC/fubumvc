using System;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.SelfHost.Testing
{
    [TestFixture]
    public class HeaderKeyValuesTester
    {
        private HttpRequestHeaders theHeaders;
        private HeaderKeyValues theKeyValues;

        [SetUp]
        public void SetUp()
        {
            var request = new HttpRequestMessage();
            theHeaders = request.Headers;

            theKeyValues = new HeaderKeyValues(theHeaders);
        }

        [Test]
        public void has_negative()
        {
            theKeyValues.Has(Core.Http.HttpRequestHeaders.Warning).ShouldBeFalse();
        }

        [Test]
        public void has_positive()
        {
            theHeaders.Add("x-extension", "737060cd8c284d8af7ad3082f209582d");
            theKeyValues.Has("x-extension").ShouldBeTrue();
        }

        [Test]
        public void get()
        {
            theHeaders.Add("x-extension", "737060cd8c284d8af7ad3082f209582d");
            theKeyValues.Get("x-extension").ShouldEqual("737060cd8c284d8af7ad3082f209582d");
        }

        [Test]
        public void get_all_keys()
        {
            theHeaders.Add("x-1", "a");
            theHeaders.Add("x-2", "a");
            theHeaders.Add("x-3", "a");
        
            theKeyValues.GetKeys().ShouldHaveTheSameElementsAs("x-1", "x-2", "x-3");
        }

        [Test]
        public void ForValue_miss()
        {
            theKeyValues.ForValue("x-1", (x, y) => Assert.Fail("should not be here")).ShouldBeFalse();
        }

        [Test]
        public void ForValue_positive()
        {
            theHeaders.Add("x-1", "a");

            theKeyValues.ForValue("x-1", (key, value) =>
            {
                value.ShouldEqual("a");
                key.ShouldEqual("x-1");
            }).ShouldBeTrue();
        }
    }
}