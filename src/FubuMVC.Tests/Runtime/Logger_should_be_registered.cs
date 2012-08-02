using FubuCore.Logging;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Runtime.Logging
{
    [TestFixture]
    public class Logger_should_be_registered
    {
        [Test]
        public void Logger_should_be_registered_as_a_core_service()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<ILogger>()
                .Type.ShouldEqual(typeof (Logger));
        }
    }
}