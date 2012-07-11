using FubuCore;
using FubuMVC.Core.Runtime.Logging;
using StructureMap.AutoMocking;

namespace FubuMVC.Tests
{
    public static class InteractionContextExtensions
    {
        public static void RecordLogging<T>(this RhinoAutoMocker<T> mocker) where T : class
        {
            mocker.Inject<ILogger>(new RecordingLogger());
        }

        public static RecordingLogger RecordedLog<T>(RhinoAutoMocker<T> mocker) where T : class
        {
            return mocker.Get<ILogger>().As<RecordingLogger>();
        }
    }
}