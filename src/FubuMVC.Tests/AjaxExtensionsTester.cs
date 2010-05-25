using System;
using System.Collections.Generic;
using System.Web;
using NUnit.Framework;
using FubuMVC.Core;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class AjaxExtensionsTester
    {
        private HttpContext _ajaxContext;
        private HttpContext _nonAjaxContext;
        private IDictionary<string, object> _ajaxRequestInput = new Dictionary<string, object> { { "X-Requested-With", "XMLHttpRequest" } };
        private IDictionary<string, object> _nonAjaxRequestInput = new Dictionary<string, object> { { "X-Requested-With", "some_value" } };

        [SetUp]
        public void SetUp()
        {
            var request = new HttpRequest("foo.txt", "http://test", "X-Requested-With=XMLHttpRequest");
            _ajaxContext = new HttpContext(request, new HttpResponse(Console.Out));
            var nonAjaxRequest = new HttpRequest("foo.txt", "http://test", "X-Requested-With=some_value");
            _nonAjaxContext = new HttpContext(nonAjaxRequest, new HttpResponse(Console.Out));
        }

        [Test]
        public void is_http_context_an_ajax_request()
        {
            _ajaxContext.IsAjaxRequest().ShouldBeTrue();
            _nonAjaxContext.IsAjaxRequest().ShouldBeFalse();
        }

        [Test]
        public void is_dictionary_input_an_ajax_request()
        {
            _ajaxRequestInput.IsAjaxRequest().ShouldBeTrue();
            _nonAjaxRequestInput.IsAjaxRequest().ShouldBeFalse();
        }
    }
}