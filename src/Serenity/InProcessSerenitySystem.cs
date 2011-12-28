using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.OwinHost;
using StoryTeller.Engine;

namespace Serenity
{
    public class InProcessSerenitySystem<TSystem> : BasicSystem where TSystem : IApplicationSource, new()
    {
        private IApplicationUnderTest _application;

        protected virtual ApplicationSettings findApplicationSettings()
        {
            return ApplicationSettings.ReadFor<TSystem>();
        }

        public override void RegisterServices(ITestContext context)
        {
            if (_application != null)
            {
                context.Store(_application);
                context.Store(new NavigationDriver(_application));
            }
        }

        public override sealed void Setup()
        {
            if (_application == null)
            {
                var settings = findApplicationSettings();
                settings.Port = PortFinder.FindPort(settings.Port);
                settings.RootUrl = "http://localhost:" + settings.Port;

                _application = new InProcessApplicationUnderTest<TSystem>(settings);
            }

            beforeExecutingTest(_application);
        }


        protected virtual void beforeExecutingTest(IApplicationUnderTest application)
        {
        }

        public T Get<T>()
        {
            return _application.GetInstance<T>();
        }

        public override object Get(Type type)
        {
            if (type == typeof (IApplicationUnderTest))
            {
                return _application;
            }

            var getterType = typeof (Getter<>).MakeGenericType(type);
            var getter = Activator.CreateInstance(getterType, _application).As<IGetter>();

            return getter.Get();
        }

        public override sealed void TeardownEnvironment()
        {
            if (_application != null) _application.Teardown();

            shutDownSystem();
        }

        protected virtual void shutDownSystem()
        {
        }

        public class Getter<T> : IGetter
        {
            private readonly IApplicationUnderTest _container;

            public Getter(IApplicationUnderTest container)
            {
                _container = container;
            }

            public object Get()
            {
                return _container.GetInstance<T>();
            }
        }

        public interface IGetter
        {
            object Get();
        }

    }
}