using NUnit.Framework;

namespace FubuMVC.Media.Testing
{
    [TestFixture]
    public class default_service_registrations
    {
        [Test]
        public void fix_these()
        {
            Assert.Fail("Do.");
        }


        /*
        [Test]
        public void value_source_is_registered()
        {
            var services = new FubuRegistry().BuildGraph().Services;
            services.DefaultServiceFor(typeof (IValueSource<>)).Type.ShouldEqual(typeof (ValueSource<>));
        }

        [Test]
        public void values_is_registered()
        {
            var services = new FubuRegistry().BuildGraph().Services;
            services.DefaultServiceFor(typeof (IValues<>)).Type.ShouldEqual(typeof (SimpleValues<>));
        }



        [Test]
        public void projection_runner_is_registered()
        {
            registeredTypeIs<IProjectionRunner, ProjectionRunner>();
        }

        [Test]
        public void generic_projection_runner_is_registered()
        {
            var services = new FubuRegistry().BuildGraph().Services;
            services.DefaultServiceFor(typeof(IProjectionRunner<>)).Type.ShouldEqual(typeof(ProjectionRunner<>));
        }
         */
    }
}