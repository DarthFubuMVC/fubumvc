using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures;
using Serenity.Testing.Fixtures.Grammars;
using StoryTeller;
using StoryTeller.Domain;
using TestContext = StoryTeller.Engine.TestContext;

namespace Serenity.Testing.Fixtures
{
    [TestFixture]
    public class BrowserIsAtTester : ScreenFixture
    {
        public class FakeApplication : IApplicationUnderTest
        {
            public FakeApplication()
            {
                Driver = new FakeWebDriver();
                Urls = new StubHttpUrlRegistry();
            }

            public string Name { get; private set; }
            public IUrlRegistry Urls { get; private set; }
            public IWebDriver Driver { get; private set; }
            public string RootUrl { get; private set; }
            public IServiceLocator Services { get; private set; }
            public IBrowserLifecycle Browser { get; private set; }
            public void Ping()
            {
                throw new NotImplementedException();
            }

            public void Teardown()
            {
                throw new NotImplementedException();
            }

            public NavigationDriver Navigation { get; private set; }
            public EndpointDriver Endpoints()
            {
                return new EndpointDriver(Urls);
            }
        }
        public class FakeWebDriver : IWebDriver
        {
            public IWebElement FindElement(By @by)
            {
                throw new NotImplementedException();
            }

            public ReadOnlyCollection<IWebElement> FindElements(By @by)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void Close()
            {
                throw new NotImplementedException();
            }

            public void Quit()
            {
                throw new NotImplementedException();
            }

            public IOptions Manage()
            {
                throw new NotImplementedException();
            }

            public INavigation Navigate()
            {
                throw new NotImplementedException();
            }

            public ITargetLocator SwitchTo()
            {
                throw new NotImplementedException();
            }

            public string Url { get; set; }
            public string Title { get; private set; }
            public string PageSource { get; private set; }
            public string CurrentWindowHandle { get; private set; }
            public ReadOnlyCollection<string> WindowHandles { get; private set; }
        }
        public class FakeInputModel { }
        public class FakeViewModel { }
        public class FakeEndpoint
        {
            public FakeViewModel get_home()
            {
                return new FakeViewModel();
            }
        }

        private IUrlRegistry theUrlRegistry;

        [SetUp]
        public void SetUp()
        {
            Context = new TestContext();
            var application = new FakeApplication();
            Context.Store<IApplicationUnderTest>(application);
            SetUp(Context);
            theUrlRegistry = application.Urls;
        }

        [Test]
        public void browserisat_urlfor_by_model_is_right()
        {
            Application.Driver.Url = theUrlRegistry.UrlFor(new FakeInputModel());
            IsRight(BrowserIsAt(new FakeInputModel(), "title"));
        }

        [Test]
        public void browserisat_urlfor_by_model_get_post_count_a_wrong()
        {
            Application.Driver.Url = theUrlRegistry.UrlFor(new FakeInputModel(), "GET");
            IsWrong(BrowserIsAt(new FakeInputModel(), "title", "POST"));
        }
        
        [Test]
        public void browserisat_urlfor_by_T_counts_right()
        {
            Application.Driver.Url = theUrlRegistry.UrlFor<FakeInputModel>();
            IsRight(BrowserIsAt<FakeInputModel>("title"));
        }

        [Test]
        public void browserisat_urlfor_by_type_works()
        {
            Application.Driver.Url = theUrlRegistry.UrlFor(typeof (FakeInputModel));
            IsRight(BrowserIsAt(typeof (FakeInputModel), "title"));
        }

        [Test]
        public void browserisat_urlfor_by_type_using_different_types()
        {
            Application.Driver.Url = theUrlRegistry.UrlFor(typeof(FakeInputModel));
            IsWrong(BrowserIsAt(typeof(FakeViewModel), "title"));
        }

        [Test]
        public void browserisat_urlfor_by_endpoint_method_works()
        {
            Application.Driver.Url = theUrlRegistry.UrlFor<FakeEndpoint>(x => x.get_home());
            IsRight(BrowserIsAt<FakeEndpoint>(x => x.get_home(), "title"));
        }

        [Test]
        public void browserisat_matching_url_counts_right()
        {
            var url = theUrlRegistry.UrlFor(new FakeInputModel());
            Application.Driver.Url = url;
            IsRight(BrowserIsAt(x => url, "title"));
        }

        [Test]
        public void browserisat_counts_wrong_when_url_does_not_match()
        {
            var url = theUrlRegistry.UrlFor(new FakeInputModel());
            Application.Driver.Url = "//wrongurl";
            IsWrong(BrowserIsAt(x => url, "title"));
        }

        private static void IsRight(IGrammar grammar)
        {
            var result = grammar.Execute();
            result.Counts.Rights.ShouldEqual(1);
            result.Counts.Wrongs.ShouldEqual(0);
        }

        private static void IsWrong(IGrammar grammar)
        {
            var result = grammar.Execute();
            result.Counts.Rights.ShouldEqual(0);
            result.Counts.Wrongs.ShouldEqual(1);
        }

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
            return _baseAddress + _stubRegistry.UrlFor<TController>(expression, categoryOrHttpMethod);
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