using System;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Logging;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.TestSupport
{
    public class InteractionContext<T> where T : class
    {
        private SettableClock _clock;



        public IContainer Container { get { return Services.Container; } }
        public RhinoAutoMocker<T> Services { get; private set; }
        public T ClassUnderTest { get { return Services.ClassUnderTest; } }

        [SetUp]
        public void SetUp()
        {
            _clock = new SettableClock();

            Services = new RhinoAutoMocker<T>();
            Services.Inject<ISystemTime>(_clock);
            beforeEach();
        }

        public RecordingLogger RecordLogging()
        {
            var logger = new RecordingLogger();
            Services.Inject<ILogger>(logger);

            return logger;
        }

        public RecordingLogger RecordedLog()
        {
            return MockFor<ILogger>().As<RecordingLogger>();
        }

        // Override this for context specific setup
        protected virtual void beforeEach() {}

        public TService MockFor<TService>() where TService : class
        {
            return Services.Get<TService>();
        }

        public void VerifyCallsFor<TMock>() where TMock : class
        {
            MockFor<TMock>().VerifyAllExpectations();
        }

        public DateTime LocalSystemTime
        {
            get
            {
                return _clock.LocalTime().Time;
            }
            set
            {
                _clock.LocalNow(value);
            }
        }

        public LocalTime LocalTime
        {
            get
            {
                return _clock.LocalTime();
            }
        }

        public DateTime UtcSystemTime
        {
            get
            {
                return _clock.UtcNow();
            }
        }
    }
}