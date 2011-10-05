using System;
using System.Collections.Generic;
using FubuMVC.Core.Urls;
using StoryTeller.Engine;

namespace Serenity
{



    public class FubuMvcSystem : BasicSystem
    {
        private readonly IEnumerable<IApplicationUnderTest> _applications;
        // The first one should be considered to be the main one
        public FubuMvcSystem(params IApplicationUnderTest[] applications)
        {
            _applications = applications;
        }

        //public override object Get(Type type)
        //{
        //    return 
        //}

        public override void RegisterServices(ITestContext context)
        {
            throw new NotImplementedException();
        }

        public override void SetupEnvironment()
        {
            // Need to spin up the browser
            _applications.Each(x => x.Ping());
        }

        public override void TeardownEnvironment()
        {
            throw new NotImplementedException();
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }

        public override void Teardown()
        {
            throw new NotImplementedException();
        }

        public override void RegisterFixtures(FixtureRegistry registry)
        {
            throw new NotImplementedException();
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

        // Whatever you want to happen to start.  Nah, make it lazy

        // Whatever you want cleaned up.
        // Prefer to do this by just registering 
    }

    // This will be used to set up the application
    public class WebDriverSettings
    {
        public string BrowserName { get; set;}
    }

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