using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http.AspNet;
using FubuTestingSupport;
using NUnit.Framework;
using FubuMVC.Core;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class AjaxExtensionsTester
    {

        private IDictionary<string, object> _ajaxRequestInput = new Dictionary<string, object> { { "X-Requested-With", "XMLHttpRequest" } };
        private IDictionary<string, object> _nonAjaxRequestInput = new Dictionary<string, object> { { "X-Requested-With", "some_value" } };

        [Test]
        public void is_dictionary_input_an_ajax_request()
        {
            _ajaxRequestInput.IsAjaxRequest().ShouldBeTrue();
            _nonAjaxRequestInput.IsAjaxRequest().ShouldBeFalse();
        }

        [Test]
        public void x()
        {
            var collection = new NameValueCollection();
            collection.Add("x-requested-with","XMLHttpRequest");
            collection["X-Requested-With"].ShouldEqual("XMLHttpRequest");


            var requestData = new RequestData();
            requestData.AddValues(new FlatValueSource<object>(new SimpleKeyValues(key => collection[key],() => collection.AllKeys)));
            requestData.IsAjaxRequest().ShouldBeTrue();
        }
    }
}