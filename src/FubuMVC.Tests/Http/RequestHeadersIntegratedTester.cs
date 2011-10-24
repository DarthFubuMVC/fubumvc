using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.StructureMap;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class RequestHeadersIntegratedTester
    {
        private Dictionary<string, object> theHeaderDictionary;
        private RequestHeaders theHeaders;

        [SetUp]
        public void SetUp()
        {
            var container = StructureMapContainerFacility.GetBasicFubuContainer();
            theHeaderDictionary = new Dictionary<string, object>();
            var dictionary = new AggregateDictionary();
            dictionary.AddDictionary(RequestDataSource.Header.ToString(), theHeaderDictionary);

            container.Inject(dictionary);

            theHeaders = container.GetInstance<RequestHeaders>();
        }

        [Test]
        public void value_negative()
        {
            theHeaderDictionary.ContainsKey(HttpRequestHeaders.IfNoneMatch).ShouldBeFalse();
            
            var action = MockRepository.GenerateMock<Action<string>>();
            theHeaders.Value(HttpRequestHeaders.IfNoneMatch, action);

            action.AssertWasNotCalled(x => x.Invoke(null), x => x.IgnoreArguments());

        }

        [Test]
        public void value_positive()
        {
            theHeaderDictionary.Add(HttpRequestHeaders.IfNoneMatch, "12345");

            var action = MockRepository.GenerateMock<Action<string>>();
            theHeaders.Value(HttpRequestHeaders.IfNoneMatch, action);

            action.AssertWasCalled(x => x.Invoke("12345"));
        }

        [Test]
        public void convert_to_something_besides_strings()
        {
            theHeaderDictionary.Add(HttpRequestHeaders.IfNoneMatch, "12345");

            var action = MockRepository.GenerateMock<Action<int>>();
            theHeaders.Value<int>(HttpRequestHeaders.IfNoneMatch, action);

            action.AssertWasCalled(x => x.Invoke(12345));
        }


        // There's a little bit of misdirection here.  This depends on a naming strategy
        // being applied to BindingContext in FubuApplication
        [Test]
        public void bind_an_object()
        {
            var modifiedSince = DateTime.Today.AddMinutes(-90);
            theHeaderDictionary.Add(HttpRequestHeaders.IfModifiedSince, modifiedSince.ToString());
            theHeaderDictionary.Add(HttpRequestHeaders.IfMatch, "12345");
            theHeaderDictionary.Add(HttpRequestHeaders.IfNoneMatch, "2345");

            var dto = theHeaders.BindToHeaders<ETagDto>();
            dto.IfModifiedSince.ShouldEqual(modifiedSince);
            dto.IfMatch.ShouldEqual("12345");
            dto.IfNoneMatch.ShouldEqual("2345");
        }

        public class ETagDto
        {
            public string IfMatch { get; set; }
            public DateTime IfModifiedSince { get; set; }
            public string IfNoneMatch { get; set; }
        }
    }
}