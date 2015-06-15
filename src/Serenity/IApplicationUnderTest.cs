using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;
using StoryTeller;

namespace Serenity
{
    public interface IApplicationUnderTest
    {
        string Name { get; }
        IUrlRegistry Urls { get; }
        IWebDriver Driver { get; }

        string RootUrl { get; }

        IServiceLocator Services { get; }

        IBrowserLifecycle Browser { get; }

        void Ping();

        void Teardown();

        NavigationDriver Navigation { get;}
        EndpointDriver Endpoints();

    }

    public static class ApplicationUnderTestExtensions
    {
        public static bool AssertIsOnScreen<T>(this IApplicationUnderTest application, T input)
        {
            var expected = application.Urls.UrlFor(input, "GET");
            var actual = application.Driver.Url;

            if (!expected.IsUrlMatch(application.Driver.Url))
            {
                StoryTellerAssert.Fail("The actual Url of the browser is " + actual.Canonize());
            }

            return true;
        }

        public static bool AssertIsOnScreen(IApplicationUnderTest application, string expected)
        {
            var actual = application.Driver.Url;

            if (!expected.IsUrlMatch(application.Driver.Url))
            {
                StoryTellerAssert.Fail("The actual Url of the browser is " + actual.Canonize());
            }

            return true;
        }

        public static bool AssertIsNotOnScreen<T>(this IApplicationUnderTest application, T input)
        {
            var actual = new Uri(application.Driver.Url).AbsolutePath;
            var expected = application.Urls.UrlFor(input, "GET");

            return !expected.IsUrlMatch(actual);
        }
    }
}