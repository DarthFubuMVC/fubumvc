using System;
using System.Threading;
using FubuCore;
using FubuKayak;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration.ObjectGraph;
using OpenQA.Selenium;
using StoryTeller.Engine;

namespace Serenity
{
    public class InProcessSerenitySystem<TSystem> : BasicSystem where TSystem : IApplicationSource, new()
    {
        private readonly Func<IWebDriver> _browserBuilder;
        private Listener _listener;
        private ApplicationUnderTest _application;
        private ManualResetEvent _reset;


        public InProcessSerenitySystem()
        {
            _browserBuilder = WebDriverSettings.DriverBuilder();
        }

        protected virtual ApplicationSettings findApplicationSettings()
        {
            return ApplicationSettings.ReadFor<TSystem>();
        }

        public override void RegisterServices(ITestContext context)
        {
            if (_application != null)
            {
                context.Store<IApplicationUnderTest>(_application);
                context.Store(new ApplicationDriver(_application));
            }
        }

        public sealed override void Setup()
        {
            if (_application == null)
            {
                var settings = findApplicationSettings();
                FubuMvcPackageFacility.PhysicalRootPath = settings.GetApplicationFolder();

                // TODO -- add some diagnostics here
                var runtime = new TSystem().BuildApplication().Bootstrap();
                runtime.Facility.Register(typeof (ICurrentHttpRequest), ObjectDef.ForValue(new StubCurrentHttpRequest(){
                    ApplicationRoot = "http://localhost:" + settings.Port
                }));

                _listener = new Listener(settings.Port);
                _reset = _listener.StartOnNewThread(runtime, () => { });

                settings.RootUrl = "http://localhost:" + settings.Port;

                _application = new ApplicationUnderTest(runtime, settings, _browserBuilder);

                _reset.WaitOne();
            }

            beforeExecutingTest(_application);
        }

        protected virtual void beforeExecutingTest(ApplicationUnderTest application)
        {
                
        }

        public T Get<T>()
        {
            return _application.GetInstance<T>();
        }

        public override object Get(Type type)
        {
            if (type == typeof(IApplicationUnderTest))
            {
                return _application;
            }
            
            var getterType = typeof (Getter<>).MakeGenericType(type);
            var getter = Activator.CreateInstance(getterType, _application).As<IGetter>();

            return getter.Get();
        }

        public interface IGetter
        {
            object Get();
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

        public sealed override void TeardownEnvironment()
        {
            if (_application != null) _application.Teardown();
            if (_listener != null)
            {
                _listener.Stop();
                _listener.SafeDispose();
            }

            shutDownSystem();
        }

        protected virtual void shutDownSystem()
        {
            
        }
    }
}