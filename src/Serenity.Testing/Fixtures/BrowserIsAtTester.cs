using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using Shouldly;
using NUnit.Framework;
using Serenity.Fixtures;

namespace Serenity.Testing.Fixtures
{
    [TestFixture]
    public class BrowserIsAtTester : ScreenFixture
    {
        [Test]
        public void canonize()
        {
            "http://localhost".Canonize().ShouldBe("/");
            "http://localhost:5500".Canonize().ShouldBe("/");
            "http://localhost:5500/a".Canonize().ShouldBe("/a");
            "/a".Canonize().ShouldBe("/a");

            "http://host.com/a/b".Canonize().ShouldBe("/a/b");
        }

        [Test]
        public void is_exact_url_match()
        {

            "http://localhost:5500/?a=b".MatchesWithQuerystring("/?a=b").ShouldBeTrue();
            "http://localhost:5500/?a=b".MatchesWithQuerystring("/?a=c").ShouldBeFalse();


        }

        [Test]
        public void is_url_match()
        {
            "http://localhost:5500".Matches("/").ShouldBeTrue();
            "http://localhost:5500/".Matches("/").ShouldBeTrue();
            "http://localhost:5500".Matches("/?a=b").ShouldBeTrue();
            "http://localhost:5500/home?a=b".Matches("/").ShouldBeFalse();


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

        public string UrlFor(Type handlerType, MethodInfo method, string categoryOrHttpMethodOrHttpMethod)
        {
            return _baseAddress + _stubRegistry.UrlFor(handlerType, method, categoryOrHttpMethodOrHttpMethod);
        }
    }
}