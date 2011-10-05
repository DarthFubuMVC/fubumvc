using System;
using System.Collections.Generic;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;

namespace Serenity
{
    public class ApplicationUnderTest : IApplicationUnderTest
    {
        private readonly IApplicationSource _source;
        private readonly ApplicationSettings _settings;
        private readonly Lazy<IWebDriver> _driver;
        private readonly Lazy<IContainerFacility> _container;
        private readonly Lazy<IUrlRegistry> _urls;

        public ApplicationUnderTest(IApplicationSource source, ApplicationSettings settings, Func<IWebDriver> createWebDriver)
        {
            _source = source;
            _settings = settings;
            _driver = new Lazy<IWebDriver>(createWebDriver);

            _container = new Lazy<IContainerFacility>(() =>
            {
                var app = _source.BuildApplication();
                app.Bootstrap();

                return app.Facility;
            });

            _urls = new Lazy<IUrlRegistry>(() =>
            {
                var urls = GetInstance<IUrlRegistry>();
                urls.As<UrlRegistry>().RootAt(_settings.RootUrl);
                return urls;
            });
        }

        public string Name
        {
            get { return _source.Name; }
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
            client.DownloadDataAsync(new Uri(_settings.RootUrl));
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
}