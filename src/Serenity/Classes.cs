using System;
using System.Collections.Generic;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;
using StoryTeller.Engine;

namespace Serenity
{



    public abstract class FubuMvcSystem : BasicSystem
    {
        private readonly IList<IApplicationUnderTest> _applications = new List<IApplicationUnderTest>();
        // The first one should be considered to be the main one
        
        public void AddApplication(IApplicationUnderTest application)
        {
            _applications.Add(application);
        }

        //public override object Get(Type type)
        //{
        //    return 
        //}

        public override void RegisterServices(ITestContext context)
        {
            // TODO -- does need to push in the IApplicationUnderTest
            throw new NotSupportedException();
        }

        public override void SetupEnvironment()
        {
            // Need to spin up the browser
            _applications.Each(x => x.Ping());
        }

        public override void TeardownEnvironment()
        {
            _applications.Each(x => x.Teardown());
        }




    }


    // TODO -- figure out how to ping it
    public interface IApplicationUnderTest
    {
        string Name { get; }

        T GetInstance<T>();
        IEnumerable<T> GetAll<T>();

        IUrlRegistry Urls { get; }

        void Ping();

        void Teardown();

        IWebDriver Driver { get; }
    }


    // This will be used to set up the application
    public class WebDriverSettings
    {
        public string BrowserName { get; set;}
    }
    
    // An application
    public class ApplicationSettings
    {
        public string Name { get; set; }
        public string PhysicalPath { get; set; }
        public string RootUrl { get; set; }
    }

    public class ScreenFixture : Fixture
    {
        
    }

    public abstract class ScreenFixture<T> : Fixture
    {
        
    }
}