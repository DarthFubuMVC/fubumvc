using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class RequestDataTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            dictionary = new Dictionary<string, object>();
            aggregate = new AggregateDictionary();
            aggregate.AddDictionary("Other", dictionary);


            request = new RequestData(aggregate);
        }

        #endregion

        private Dictionary<string, object> dictionary;
        private RequestData request;
        private AggregateDictionary aggregate;

        [Test]
        public void call_into_the_continuation_with_the_dictionary_value_if_it_exists()
        {
            string name = null;

            dictionary.Add("name", "Something");

            request.Value("name", o => name = (string) o);

            name.ShouldEqual("Something");
        }

        [Test]
        public void do_nothing_when_the_value_is_not_found()
        {
            request.Value("name", o => { throw new NotImplementedException(); });
        }
    }
}