using System;
using System.Collections.Generic;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;
using Serenity.Endpoints;

namespace Serenity
{
    public interface IApplicationUnderTest
    {
        string Name { get; }
        IUrlRegistry Urls { get; }
        IWebDriver Driver { get; }

        string RootUrl { get; }

        T GetInstance<T>();
        object GetInstance(Type type);
        IEnumerable<T> GetAll<T>();

        // TODO -- don't care for this at all.  Needs to be 
        // encapsulated into ApplicationUnderTest itself
        void StartWebDriver();
        void StopWebDriver();
        bool IsDriverInUse { get; }

        void Ping();

        void Teardown();

        NavigationDriver Navigation { get;}
        EndpointDriver Endpoints();

    }
}