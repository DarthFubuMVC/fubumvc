using System;
using System.Collections.Generic;
using FubuMVC.Core;
using OpenQA.Selenium;
using StoryTeller.Engine;

namespace Serenity
{
    public class SerenitySystem : BasicSystem
    {
        private readonly SerenityApplications _applications = new SerenityApplications();
        private readonly Func<IWebDriver> _browserBuilder;

        public SerenitySystem()
        {
            _browserBuilder = WebDriverSettings.Read().DriverBuilder();
        }

        public SerenityApplications Applications
        {
            get { return _applications; }
        }

        public void AddApplication(IApplicationSource source)
        {
            var settings = ApplicationSettings.ReadByName(source.Name);
            var application = new ApplicationUnderTest(source, settings, _browserBuilder);
            _applications.AddApplication(application);
        }

        // TODO -- have a way to do this inline
        public void AddApplication<T>() where T : IApplicationSource, new()
        {
            AddApplication(new T());
        }

        public override void RegisterServices(ITestContext context)
        {
            context.Store(_applications);
            context.Store(_applications.PrimaryApplication());
        }

        public override void SetupEnvironment()
        {
            _applications.Each(x => x.Ping());
        }

        public override void TeardownEnvironment()
        {
            _applications.Each(x => x.Teardown());
        }
    }
}