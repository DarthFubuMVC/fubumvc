using System.Collections.Generic;
using System.Collections.Specialized;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Cookies
{
	[TestFixture]
	public class CookiesTester
	{
		private StubCurrentHttpRequest theRequest;
		private Core.Http.Cookies.Cookies theCookies;

		[SetUp]
		public void SetUp()
		{
			theRequest = new StubCurrentHttpRequest();
			theCookies = new Core.Http.Cookies.Cookies(theRequest);
		}

		[Test]
		public void single_cookie()
		{
			theRequest.Data[HttpRequestHeaders.Cookie] = "a=123;";
			theCookies.Get("a").Value.ShouldEqual("123");
		}

		[Test]
		public void multiple_cookies()
		{
			theRequest.Data[HttpRequestHeaders.Cookie] = "a=123;b=456;";
			theCookies.Get("a").Value.ShouldEqual("123");
			theCookies.Get("b").Value.ShouldEqual("456");
		}

		public class StubCurrentHttpRequest : ICurrentHttpRequest
		{
			public string RawUrl()
			{
				throw new System.NotImplementedException();
			}

			public string RelativeUrl()
			{
				throw new System.NotImplementedException();
			}

			public string FullUrl()
			{
				throw new System.NotImplementedException();
			}

			public string ToFullUrl(string url)
			{
				throw new System.NotImplementedException();
			}

			public string HttpMethod()
			{
				throw new System.NotImplementedException();
			}

			public bool HasHeader(string key)
			{
				throw new System.NotImplementedException();
			}

			public KeyValues Data = new KeyValues();

			public IEnumerable<string> GetHeader(string key)
			{
				return new[] {Data.Get(key)};
			}

			public IEnumerable<string> AllHeaderKeys()
			{
				throw new System.NotImplementedException();
			}

		    public NameValueCollection QueryString { get; private set; }
		}
	}
}