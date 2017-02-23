using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication
{
	
	public class AjaxAuthenticationRedirectTester
	{
		private AjaxAuthenticationRedirect theRedirect;
		private IRequestData theRequestData;

	    public AjaxAuthenticationRedirectTester()
	    {
			theRequestData = new InMemoryRequestData();
			theRedirect = new AjaxAuthenticationRedirect(theRequestData, null, null);
		}

		[Fact]
		public void applies_to_ajax_requests_positive()
		{
			var data = new Dictionary<string, string>();
			data.Add(AjaxExtensions.XRequestedWithHeader, AjaxExtensions.XmlHttpRequestValue);
			theRequestData.AddValues("test", new DictionaryKeyValues(data));

			theRedirect.Applies().ShouldBeTrue();
		}

		[Fact]
		public void applies_to_ajax_requests_negative()
		{
			theRedirect.Applies().ShouldBeFalse();
		}
	}
}