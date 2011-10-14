using System.Collections.Generic;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;

namespace Serenity
{
    public interface IApplicationUnderTest
    {
        string Name { get; }
        IUrlRegistry Urls { get; }
        IWebDriver Driver { get; }

        string RootUrl { get; }

        T GetInstance<T>();
        IEnumerable<T> GetAll<T>();

        void Ping();

        void Teardown();
    }
}