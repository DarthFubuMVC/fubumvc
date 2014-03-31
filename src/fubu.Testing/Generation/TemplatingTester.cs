using System.Linq;
using Fubu.Generation;
using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Graph;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Generation
{
    [TestFixture]
    public class TemplatingTester
    {
        [Test]
        public void should_add_a_bundler_step_if_there_are_any_gem_references()
        {
            var request = new TemplateRequest
            {
                SolutionName = "Foo",
                RootDirectory = "Foo"
            };

            request.AddTemplate("baseline");

            Templating.BuildPlan(request)
                      .Steps.OfType<BundlerStep>()
                      .Count().ShouldEqual(1);
        }

        [Test]
        public void can_load_the_library_with_graph()
        {
            Templating.Library.ShouldNotBeNull();
            Templating.Library.Graph.ShouldNotBeNull();
            Templating.Library.Find(TemplateType.Solution, "baseline").ShouldNotBeNull();
            Templating.Library.Graph.FindCategory("new").FindTemplate("web-app")
                .ShouldNotBeNull();
        }
    }
}