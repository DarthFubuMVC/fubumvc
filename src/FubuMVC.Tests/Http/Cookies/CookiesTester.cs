using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Cookies
{
	[TestFixture]
	public class CookiesTester
	{
		private OwinHttpRequest theRequest;
		private Core.Http.Cookies.Cookies theCookies;

		[SetUp]
		public void SetUp()
		{
			theRequest = OwinHttpRequest.ForTesting();
			theCookies = new Core.Http.Cookies.Cookies(theRequest);
		}

		[Test]
		public void single_cookie()
		{
		    theRequest.AppendHeader(HttpRequestHeaders.Cookie, "a=123;");
			theCookies.Get("a").Value.ShouldBe("123");
		}

		[Test]
		public void multiple_cookies()
		{
            theRequest.AppendHeader(HttpRequestHeaders.Cookie, "a=123;b=456;");
			theCookies.Get("a").Value.ShouldBe("123");
			theCookies.Get("b").Value.ShouldBe("456");
		}

	}
}