using System;
using System.Collections.Generic;
using System.Web;
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
    }
}