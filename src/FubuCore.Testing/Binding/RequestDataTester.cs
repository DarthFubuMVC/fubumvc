using System.Collections.Generic;
using FubuCore.Binding;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class RequestDataTester
    {
        private const string _expectedValue = "STUBBED USERAGENT";

        public interface ICallback {void Action(object o);}

        [Test]
        public void for_dictionary_returns_new_request_data_with_added_dictionary()
        {
            var callback = MockRepository.GenerateStub<ICallback>();
            IDictionary<string, object> dictionary = new Dictionary<string, object> { { "UserAgent", _expectedValue } };
            RequestData data = RequestData.ForDictionary(dictionary);
            data.ShouldNotBeNull();
            data.Value("UserAgent", callback.Action);
            callback.AssertWasCalled(c=>c.Action(_expectedValue));
        }

        [Test]
        public void value_returns_value_for_key()
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object> { { "UserAgent", _expectedValue } };
            RequestData data = RequestData.ForDictionary(dictionary);
            data.Value("UserAgent").ShouldEqual(_expectedValue);
        }
    }
}