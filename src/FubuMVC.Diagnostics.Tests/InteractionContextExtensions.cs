using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.Runtime.Logging;
using FubuMVC.Core.Urls;
using StructureMap.AutoMocking;

namespace FubuMVC.Diagnostics.Tests
{
    public static class InteractionContextExtensions
    {
        // TODO -- put this into FubuMVC.TestingSupport or whatever it's called
        public static StubUrlRegistry StubUrls<T>(this RhinoAutoMocker<T> mocker) where T : class
        {
            var urls = new StubUrlRegistry();
            mocker.Inject<IUrlRegistry>(urls);

            return urls;
        }

        public static RecordingLogger RecordLogging<T>(this RhinoAutoMocker<T> mocker) where T : class
        {
            var logger = new RecordingLogger();
            mocker.Inject<ILogger>(logger);

            return logger;
        }

        public static RecordingLogger RecordedLog<T>(this RhinoAutoMocker<T> mocker) where T : class
        {
            return mocker.Get<ILogger>().As<RecordingLogger>();
        }
    }
}