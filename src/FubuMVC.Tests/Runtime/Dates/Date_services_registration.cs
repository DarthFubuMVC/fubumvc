using FubuCore.Dates;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Runtime.Dates
{
    [TestFixture]
    public class Date_services_registration
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        [Test]
        public void clock_is_registered_as_a_singleton()
        {
            registeredTypeIs<IClock, Clock>();
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<IClock>().IsSingleton
                .ShouldBeTrue();
        }

        [Test]
        public void itimezone_context()
        {
            registeredTypeIs<ITimeZoneContext, MachineTimeZoneContext>();
        }

        [Test]
        public void ISystemTime()
        {
            registeredTypeIs<ISystemTime, SystemTime>();
        }
    }
}