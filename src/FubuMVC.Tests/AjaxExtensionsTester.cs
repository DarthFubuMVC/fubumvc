using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http.AspNet;
using Shouldly;
using Xunit;
using FubuMVC.Core;

namespace FubuMVC.Tests
{
    
    public class AjaxExtensionsTester
    {

        private IDictionary<string, object> _ajaxRequestInput = new Dictionary<string, object> { { "X-Requested-With", "XMLHttpRequest" } };
        private IDictionary<string, object> _nonAjaxRequestInput = new Dictionary<string, object> { { "X-Requested-With", "some_value" } };

        [Fact]
        public void is_dictionary_input_an_ajax_request()
        {
            _ajaxRequestInput.IsAjaxRequest().ShouldBeTrue();
            _nonAjaxRequestInput.IsAjaxRequest().ShouldBeFalse();
        }

    }
}