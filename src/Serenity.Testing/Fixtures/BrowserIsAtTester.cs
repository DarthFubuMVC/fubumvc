using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Serenity.Fixtures;

namespace Serenity.Testing.Fixtures
{
    [TestFixture]
    public class BrowserIsAtTester : ScreenFixture
    {
        [Test]
        public void isurlmatch_returns_true_with_matching_url()
        {
            IsUrlMatch("http://any.html", "http://any.html").ShouldBeTrue();
        }

        [Test]
        public void isurlmatch_returns_true_with_matching_text()
        {
            IsUrlMatch("//abc", "//abc").ShouldBeTrue();
        }

        [Test]
        public void isurlmatch_returns_true_with_matching_text_of_different_case()
        {
            IsUrlMatch("//aBc", "//AbC").ShouldBeTrue();
        }

        [Test]
        public void isurlmatch_returns_true_starting_with_matching_url()
        {
            IsUrlMatch("http://any:3030/thing/", "http://any:5050/").ShouldBeTrue();
        }

        [Test]
        public void irulmatch_returns_false_when_browser_url_starting_with()
        {
            IsUrlMatch("http://any/html/", "http://any/html/a").ShouldBeFalse();
        }

        [Test]
        public void isurlmatch_returns_false_when_they_dont_match()
        {
            IsUrlMatch("http://any.html", "http://other.html").ShouldBeFalse();
        }

        [Test]
        public void isurlmatch_returns_false_when_search_url_empty()
        {
            IsUrlMatch("http://some.html", "abc://").ShouldBeFalse();
        }

        [Test]
        public void empty_urls_should_return_true()
        {
            IsUrlMatch("abc://", "efg://").ShouldBeTrue();
        }

        [Test]
        public void searching_in_empty_url_returns_false()
        {
            IsUrlMatch("abc://", "http://a").ShouldBeFalse();
        }

        [Test]
        public void uri_format_in_browserurl_returns_true()
        {
            IsUrlMatch("http://something", "//something").ShouldBeTrue();
        }

        [Test]
        public void uri_format_in_browserurl_non_match_false()
        {
            IsUrlMatch("http://something", "//something/different").ShouldBeFalse();
        }

        [Test]
        public void uri_format_in_searchurl_matches()
        {
            IsUrlMatch("//something", "http://something").ShouldBeTrue();
        }

        [Test]
        public void https_uri_format_matches()
        {
            IsUrlMatch("//something", "https://something").ShouldBeTrue();
        }
    }

    public class StubHttpUrlRegistry : IUrlRegistry
    {
        public IUrlRegistry _stubRegistry = new StubUrlRegistry();
        private const string _baseAddress = "abc://local/";

        public string UrlFor<TInput>(string categoryOrHttpMethod) where TInput : class
        {
            return _baseAddress + _stubRegistry.UrlFor<TInput>(categoryOrHttpMethod);
        }

        public string UrlFor(object model, string category = null)
        {
            return _baseAddress + _stubRegistry.UrlFor(model, category);
        }

        public string UrlFor(Type modelType, RouteParameters parameters)
        {
            return _baseAddress + _stubRegistry.UrlFor(modelType, parameters);
        }

        public string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new()
        {
            throw new NotImplementedException();
        }

        public string UrlFor(Type modelType, RouteParameters parameters, string categoryOrHttpMethod)
        {
            return _baseAddress + _stubRegistry.UrlFor(modelType, parameters, categoryOrHttpMethod);
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression, string categoryOrHttpMethod)
        {
            return _baseAddress + _stubRegistry.UrlFor(expression, categoryOrHttpMethod);
        }

        public string UrlForNew(Type entityType)
        {
            return _baseAddress + _stubRegistry.UrlForNew(entityType);
        }

        public bool HasNewUrl(Type type)
        {
            throw new NotImplementedException();
        }

        public string TemplateFor(object model, string categoryOrHttpMethod = null)
        {
            throw new NotImplementedException();
        }

        public string UrlFor(Type handlerType, MethodInfo method, string categoryOrHttpMethodOrHttpMethod)
        {
            return _baseAddress + _stubRegistry.UrlFor(handlerType, method, categoryOrHttpMethodOrHttpMethod);
        }
    }
}