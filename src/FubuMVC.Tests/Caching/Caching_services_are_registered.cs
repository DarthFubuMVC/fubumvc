using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class Caching_services_are_registered
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }


    }
}