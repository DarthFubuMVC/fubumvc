using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Media.Projections;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Media.Testing
{
    [TestFixture]
    public class default_service_registrations
    {
        private ServiceGraph theServices;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Services<ResourcesServiceRegistry>();

            theServices = registry.BuildGraph().Services;
        }
        
        [Test]
        public void value_source_is_registered()
        {
            theServices.DefaultServiceFor(typeof (IValueSource<>)).Type.ShouldEqual(typeof (ValueSource<>));
        }

        [Test]
        public void values_is_registered()
        {
            theServices.DefaultServiceFor(typeof (IValues<>)).Type.ShouldEqual(typeof (SimpleValues<>));
        }



        [Test]
        public void projection_runner_is_registered()
        {
            theServices.DefaultServiceFor(typeof (IProjectionRunner)).Type.ShouldEqual(typeof (ProjectionRunner));
        }

        [Test]
        public void generic_projection_runner_is_registered()
        {
            theServices.DefaultServiceFor(typeof(IProjectionRunner<>)).Type.ShouldEqual(typeof(ProjectionRunner<>));
        }
         
    }
}