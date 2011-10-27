using FubuMVC.Core;
using FubuMVC.Core.Resources.PathBased;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Resources.PathBased
{
    [TestFixture]
    public class ResourcePathIntegratedUrlCreationTester
    {
        public class Controller1
        {
            public string get_resource(ResourcePath path)
            {
                return null;
            }
        }

        [Test]
        public void uses_the_resource_path_to_do_its_job()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            registry.BuildGraph().BehaviorFor<Controller1>(x => x.get_resource(null)).Route.CreateUrlFromInput(
                new ResourcePath("something/else"))
                .ShouldEqual("resource/something/else");
        }
    }
}