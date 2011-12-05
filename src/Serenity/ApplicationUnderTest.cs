using System;
using System.Collections.Generic;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Http;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;

namespace Serenity
{
    public class ApplicationUnderTest : IApplicationUnderTest
    {
        private readonly string _name;
        private readonly string _rootUrl;
        private readonly Lazy<IWebDriver> _driver;
        private readonly Lazy<IContainerFacility> _container;
        private readonly Lazy<IUrlRegistry> _urls;


        public ApplicationUnderTest(FubuRuntime runtime, ApplicationSettings settings, Func<IWebDriver> createWebDriver)
            : this(settings.Name, settings.RootUrl, createWebDriver, () => runtime.Facility)
        {
            
        }

        public ApplicationUnderTest(IApplicationSource source, ApplicationSettings settings, Func<IWebDriver> createWebDriver)
            : this(source.GetType().Name, settings.RootUrl, createWebDriver, () =>
            {
                var app = source.BuildApplication();

                app.ModifyRegistry(r => r.Services(x =>
                {
                    x.ReplaceService<ICurrentHttpRequest>(new StubCurrentHttpRequest
                    {
                        ApplicationRoot = settings.RootUrl
                    });
                }));

                app.Bootstrap();


                return app.Facility;
            })
        {

        }

        private ApplicationUnderTest(string name, string rootUrl, Func<IWebDriver> createWebDriver, Func<IContainerFacility> containerSource)
        {
            _name = name;
            _rootUrl = rootUrl;

            _driver = new Lazy<IWebDriver>(createWebDriver);

            _container = new Lazy<IContainerFacility>(containerSource);

            _urls = new Lazy<IUrlRegistry>(() =>
            {
                var urls = GetInstance<IUrlRegistry>();
                urls.As<UrlRegistry>().RootAt(_rootUrl);
                return urls;
            });
        }

        public string Name
        {
            get { return _name; }
        }

        public string RootUrl
        {
            get { return _rootUrl; }
        }

        public T GetInstance<T>()
        {
            return _container.Value.Get<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _container.Value.GetAll<T>();
        }

        public IUrlRegistry Urls
        {
            get { return _urls.Value; }
        }

        public void Ping()
        {
            var client = new WebClient();
            if (_rootUrl != null) client.DownloadDataAsync(new Uri(_rootUrl));
        }

        public void Teardown()
        {
            if (!_driver.IsValueCreated) return;

            Driver.Close();
            Driver.SafeDispose();
        }

        public IWebDriver Driver
        {
            get { return _driver.Value; }
        }
    }

    public class StubCurrentHttpRequest : ICurrentHttpRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string ApplicationRoot = "http://server";
        public string TheHttpMethod = "GET";

        public string RawUrl()
        {
            return TheRawUrl;
        }

        public string RelativeUrl()
        {
            return TheRelativeUrl;
        }

        public string ToFullUrl(string url)
        {
            return url.ToAbsoluteUrl(ApplicationRoot);
        }

        public string HttpMethod()
        {
            return TheHttpMethod;
        }
    }
}