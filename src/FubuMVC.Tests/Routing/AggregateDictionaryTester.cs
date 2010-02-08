using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Routing
{
    [TestFixture]
    public class AggregateDictionaryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            callback = MockRepository.GenerateMock<IDictionaryCallback>();

            dict1 = new Dictionary<string, object>
            {
                {"a", 1},
                {"b", 2},
                {"c", 3}
            };

            dict2 = new Dictionary<string, object>
            {
                {"d", 4},
                {"e", 5},
                {"f", 6}
            };

            dict3 = new Dictionary<string, object>
            {
                {"g", 7},
                {"h", 8},
                {"i", 9}
            };

            aggregate = new AggregateDictionary();
            aggregate.AddLocator(RequestDataSource.Route, key => dict1.ContainsKey(key) ? dict1[key] : null);
            aggregate.AddLocator(RequestDataSource.Request, key => dict2.ContainsKey(key) ? dict2[key] : null);
            aggregate.AddLocator(RequestDataSource.Header, key => dict3.ContainsKey(key) ? dict3[key] : null);
        }

        #endregion

        private IDictionaryCallback callback;
        private Dictionary<string, object> dict1;
        private Dictionary<string, object> dict2;
        private Dictionary<string, object> dict3;
        private AggregateDictionary aggregate;

        private void forKey(string key)
        {
            aggregate.Value(key, callback.Callback);
        }

        private void assertFound(RequestDataSource source, object value)
        {
            callback.AssertWasCalled(x => x.Callback(source, value));
        }

        [Test]
        public void find_value_from_second_dictionary()
        {
            forKey("d");
            assertFound(RequestDataSource.Request, 4);
        }

        [Test]
        public void find_value_from_third_dictionary()
        {
            forKey("i");
            assertFound(RequestDataSource.Header, 9);
        }

        [Test]
        public void find_value_from_top_dictionary()
        {
            forKey("b");
            assertFound(RequestDataSource.Route, 2);
        }

        [Test]
        public void find_value_when_it_is_not_there()
        {
            forKey("abc");

            callback.AssertWasNotCalled(x => x.Callback(RequestDataSource.Route, null), x => x.IgnoreArguments());
        }

        [Test]
        public void find_value_from_request_property()
        {
            const string expectedValue = "STUBBED USERAGENT";
            var requestCtx = Do_the_Stupid_ASPNET_Mock_HokeyPokey();
            requestCtx.HttpContext.Request.Stub(r => r.UserAgent).Return(expectedValue);
            aggregate = new AggregateDictionary(requestCtx);

            forKey("UserAgent");

            assertFound(RequestDataSource.RequestProperty, expectedValue);
        }

        private RequestContext Do_the_Stupid_ASPNET_Mock_HokeyPokey()
        {
            var context = MockRepository.GenerateStub<HttpContextBase>();
            var request = MockRepository.GenerateStub<HttpRequestBase>();
            context.Stub(c => c.Request).Return(request);
            request.Stub(r => r.Files).Return(MockRepository.GenerateStub<HttpFileCollectionBase>());
            request.Stub(r => r.Headers).Return(new NameValueCollection());
            return new RequestContext(context, new RouteData());
        }
    }

    public interface IDictionaryCallback
    {
        void Callback(RequestDataSource source, object value);
    }
}