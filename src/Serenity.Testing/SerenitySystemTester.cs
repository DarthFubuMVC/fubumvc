using FubuMVC.Core;
using NUnit.Framework;
using FubuTestingSupport;
using TestContext = StoryTeller.Engine.TestContext;

namespace Serenity.Testing
{
    [TestFixture]
    public class SerenitySystemTester
    {
        [SetUp]
        public void SetUp()
        {
            new ApplicationSettings()
            {
                Name = "Foo",
                PhysicalPath = "source/foo",
                RootUrl = "http://localhost/foo"
            }.Write();

            new ApplicationSettings()
            {
                Name = "Bar",
                PhysicalPath = "source/bar",
                RootUrl = "http://localhost/bar"
            }.Write();
        }


        [Test]
        public void add_an_application()
        {
            var system = new SerenitySystem();
            system.AddApplication(new Foo());

            var app = system.Applications.PrimaryApplication();
            app.ShouldNotBeNull();
            app.Name.ShouldEqual("Foo");
        }

        [Test]
        public void register_services_puts_both_the_serenity_applications_and_primary_app_in()
        {
            var system = new SerenitySystem();
            system.AddApplication<Foo>();
            system.AddApplication<Bar>();

            var context = new TestContext();
            system.RegisterServices(context);

            context.Retrieve<IApplicationUnderTest>().Name.ShouldEqual("Foo");
            context.Retrieve<SerenityApplications>().ShouldBeTheSameAs(system.Applications);
        }
    }

    public class Foo : FubuRegistry, IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For(this).ContainerFacility(() => null);
        }

        public override string Name
        {
            get { return "Foo"; }
        }
    }

    public class Bar : FubuRegistry, IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For(this).ContainerFacility(() => null);
        }

        public override string Name
        {
            get { return "Bar"; }
        }
    }


}