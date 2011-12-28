using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.OwinHost;
using StoryTeller.Engine;

namespace Serenity
{
    public class InProcessSerenitySystem<TSystem> : BasicSystem where TSystem : IApplicationSource, new()
    {
        private readonly Lazy<IApplicationUnderTest> _application;

        public InProcessSerenitySystem()
        {
            _application = new Lazy<IApplicationUnderTest>(() =>
            {
                var settings = findApplicationSettings();
                settings.Port = PortFinder.FindPort(settings.Port);
                settings.RootUrl = "http://localhost:" + settings.Port;

                return new InProcessApplicationUnderTest<TSystem>(settings);
            });
        }

        protected virtual ApplicationSettings findApplicationSettings()
        {
            return ApplicationSettings.ReadFor<TSystem>();
        }

        public override void RegisterServices(ITestContext context)
        {
            if (_application.IsValueCreated)
            {
                context.Store(_application.Value);
                context.Store(new NavigationDriver(_application.Value));
            }
        }

        public override sealed void Setup()
        {

            beforeExecutingTest(_application.Value);
        }


        protected virtual void beforeExecutingTest(IApplicationUnderTest application)
        {
        }

        public T Get<T>()
        {
            return _application.Value.GetInstance<T>();
        }

        public override object Get(Type type)
        {
            if (type == typeof (IApplicationUnderTest))
            {
                return _application;
            }

            return _application.Value.GetInstance(type);
        }

        public override sealed void TeardownEnvironment()
        {
            if (_application.IsValueCreated) _application.Value.Teardown();

            shutDownSystem();
        }

        protected virtual void shutDownSystem()
        {
        }


    }
}